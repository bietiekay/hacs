
if (this.wso2vis == undefined) {
	this.wso2vis = {};
};

wso2vis.ctrls = {};

wc = wso2vis.ctrls;

wc.extend = function(subc, superc) {
    if (!superc||!subc) {
        throw new Error("extend failed, please check that " + "all dependencies are included.");
    }
    var F = function() {};
    F.prototype=superc.prototype;
    subc.prototype=new F();
    subc.prototype.constructor=subc;
    subc.superclass=superc.prototype;
    if (superc.prototype.constructor == Object.prototype.constructor) {
        superc.prototype.constructor=superc;
    }
};

wc.lightcolors = {green: ["#4c7622", "#b6d76f"],
							red: ["#89080d", "#ea6949"],
							blue: ["#1f1b6f", "#7d7bd1"],
							yellow: ["#52491e", "#fdf860"],
							purple: ["#6b0544", "#f26ba6"]};

//## Base 
wc.Base = function() {
	this.attr = [];
};

wc.Base.prototype.property = function(name) {
    /*
    * Define the setter-getter globally
    */
    wc.Base.prototype[name] = function(v) {
      if (arguments.length) {
        this.attr[name] = v;
        return this;
      }
      return this.attr[name];
    };

    return this;
};

//## LED 
wc.LED = function() {
	wc.Base.call(this);
    /* @private */
    this.x(0)
        .y(0)
        .width(20)
        .height(7)
        .r(undefined)
        .color("red")
		.corner(1)
		.islit(false)
		.isshown(true)
		.smooth(true);
	/* @private */    
	this.g = undefined;
};

wc.extend(wc.LED, wc.Base);

wc.LED.prototype
    .property("x")
    .property("y")
    .property("r")
    .property("width")
    .property("height")
    .property("color")
	.property("corner")
	.property("islit")
	.property("isshown")
	.property("smooth");

wc.LED.prototype.create = function (r, x, y) {	
	t = this;
	t.r(r);
	t.x(x);
	t.y(y);	
	var color = t.islit()? wc.lightcolors[t.color()][1] : wc.lightcolors[t.color()][0];
	t.g = t.r().rect(t.x(), t.y(), t.width(), t.height(), t.corner()).attr({fill:color, stroke:"none"});
	return t;
};

wc.LED.prototype.lit = function (a, b) {
	t = this;
	t.islit(a);
	var color = t.islit()? wc.lightcolors[t.color()][1] : wc.lightcolors[t.color()][0];
	if (this.smooth()) {
		if (t.g != undefined) 
			if (b != undefined) 
				t.g.animateWith(b, {fill:color}, 100);
			else 
				t.g.animate({fill:color}, 100);
	}
	else {
		if (t.g != undefined) 
			t.g.attr({fill:color});
	}
	return this;
};

wc.LED.prototype.show = function (a) {
	this.isshown(a);
	if (a) 
		this.g.show();
	else	
		this.g.hide();
		
	return this;
};

//## Button 
wc.Button = function() {
	wc.Base.call(this);
    /* @private */
    this.x(0)
        .y(0)
        .width(60)
        .height(30)
        .r(undefined)
		.corner(5)
		.isshown(true)
		.text("POWER")
		.font(undefined)
		.fontfamily("verdana")
		.fontsize(12)
		.led(true)
		.letterspacing(20)
		.ledspacing(12);
	/* @private */    
	this.g1 = undefined;
	this.g2 = undefined;
	this.g3 = undefined;
	this.g4 = undefined;
};

wc.extend(wc.Button, wc.Base);

wc.Button.prototype
    .property("x")
    .property("y")
    .property("r")
    .property("width")
    .property("height")
	.property("corner")
	.property("isshown")
	.property("text")
	.property("led")
	.property("font")
	.property("fontfamily")
	.property("fontsize")
	.property("letterspacing")
	.property("ledspacing");

wc.Button.prototype.create = function (r, x, y) {	
	var t = this;
	this.r(r);
	this.x(x);
	this.y(y);	
	this.g1 = t.r().rect(t.x(), t.y(), t.width(), t.height(), t.corner()).attr({fill:"#666", stroke:"none"});
	this.g11 = t.r().rect(t.x(), t.y(), t.width(), t.height(), t.corner()).attr({fill:"none", stroke:"#5A5A5A", "stroke-width":3});
	if (t.font() == undefined)
		this.g2 = t.r().text(t.x() + t.width()/2, t.y() + t.height() / 2, t.text()).attr({fill:"none", stroke:"#fff", "font-family":t.fontfamily(), "font-size":t.fontsize(), 'letter-spacing':t.letterspacing()});
	else 
		this.g2 = t.r().text(t.x() + t.width()/2, t.y() + t.height() / 2, t.text()).attr({fill:"none", stroke:"#fff", font: t.font(), "line-spacing":t.letterspacing()});
	if (this.led()) {
		this.g3 = new wc.LED() .color("red");
		this.g4 = new wc.LED() .color("green");
		this.g3.create(this.r(), this.x() + this.width()/2 - this.g3.width() / 2, this.y() - this.ledspacing()) .lit(true);	
		this.g4.create(this.r(), this.x() + this.width()/2 - this.g4.width() / 2, this.y() - this.ledspacing()) .lit(false) .show(false);		   
	}
	
	var s1 = this.g1;
	var s2 = this.g2;
	var that = this;
	
	$(this.g1.node).mousedown(function() {
		s1.animate({fill:"#555"}, 0);
		s2.animateWith(s1, {stroke:"#ddd"}, 0);
	});
	
	$(this.g1.node).mouseup(function() {
		s1.animate({fill:"#666"}, 0);
		s2.animateWith(s1, {stroke:"#fff"}, 0);	
		that.onButton();
	});
	
	$(this.g2.node).mousedown(function() {
		s1.animate({fill:"#555"}, 0);
		s2.animateWith(s1, {stroke:"#ddd"}, 0);
	});
	
	$(this.g2.node).mouseup(function() {
		s1.animate({fill:"#666"}, 0);
		s2.animateWith(s1, {stroke:"#fff"}, 0);		
		that.onButton();
	});
		
	return this;
};

wc.Button.prototype.status = function (code) {
	/* 0 - standby,
	   1 - on,
	   2 - error */
	if (this.led()) {	   
		if (code == 0) {
			this.g4.show(false);
			this.g3.show(true);		
			this.g4.lit(false);
			this.g3.lit(true);		
		}
		else if (code == 1) {
			this.g4.show(true);
			this.g3.show(false);		
			this.g4.lit(true);
			this.g3.lit(false);
		}
		else if (code == 2) {
			// have a orange led
		}
	}
};

wc.Button.prototype.onButton = function () {	
};


//## LED array

wc.LEDArray = function () {
	wc.Base.call(this);
	
	this.x(10)
		.y(10)
		.length(100)
		.count(10)
		.orient("v")
		.min(0)
		.max(100)
		.orangeLevel(50)
		.redLevel(80);
	this.leds = [];
	this.cv = 0;
	this.curser = undefined;
}

wc.extend(wc.LEDArray, wc.Base);

wc.LEDArray.prototype
    .property("x")
    .property("y")
    .property("r")
    .property("length")
	.property("count")
	.property("orient")
	.property("min")
	.property("max")
	.property("orangeLevel")
	.property("redLevel");

wc.LEDArray.prototype.create = function (r, x, y) {	
	this.r(r);
	this.x(x);
	this.y(y);
	
	this.curser = this.r().circle(this.x(), this.y() + this.length(), 3).attr({stroke:"#fff", "stroke-width":2, fill:"none"})
	
	for (var i = 0; i < this.count(); i++) {
		var a = i * (this.max() - this.min()) / this.count();
		var xa = this.x() + this.length() * i / this.count();
		var ya = this.y() + this.length() - this.length() * i / this.count();
		if (a < this.orangeLevel()){
			if (this.orient() == "v") 
				this.leds.push(new wc.LED() .color("green") .smooth(false) .create(this.r(), this.x(), ya));
			else if (this.orient() == "h")
				this.leds.push(new wc.LED() .color("green") .smooth(false) .create(this.r(), xa, this.y()));
		}
		else if ((a >= this.orangeLevel()) && (a < this.redLevel())) {
			if (this.orient() == "v")
				this.leds.push(new wc.LED() .color("yellow") .smooth(false) .create(this.r(), this.x(), ya));
			else if (this.orient() == "h")
				this.leds.push(new wc.LED() .color("yellow") .smooth(false) .create(this.r(), xa, this.y()));
		}
		else if (a >= this.redLevel()) {
			if (this.orient() == "v")
				this.leds.push(new wc.LED() .color("red") .smooth(false) .create(this.r(), this.x(), ya));
			else if (this.orient() == "h")
				this.leds.push(new wc.LED() .color("red") .smooth(false) .create(this.r(), xa, this.y()));
		}
	}
	this.cv = this.y();	
	var count = this.count();
	var leds = this.leds;
	var cur = this.curser;
	this.curser.onAnimation(function () {
			for (var i = 0; i < count; i++) {
				if (leds[i].y() >= cur.attr("cy"))
					leds[i].lit(true, cur);
				else 
					leds[i].lit(false, cur);
			}
		}); 
	this.curser.hide();
	return this;
}

wc.LEDArray.prototype.update = function (val) {	
	var valCalculated = this.y() + this.length() - (val - this.min()) * this.length() / (this.max() - this.min());
	this.curser.animate({translation :"0 " + (this.cv - valCalculated)}, Math.abs((this.cv - valCalculated) * 5), "<>");
	this.cv = valCalculated;
}


//## Knob

wc.Knob = function () {
	wc.Base.call(this);
	
	this.x(10)
		.y(10)
		.minVal(0)
		.maxVal(1000)
		.largeTick(100)
		.smallTick(10)
		.minAngle(30)
		.maxAngle(330)
		.dialRadius(60)
		.ltlen(15)
		.stlen(10)
		.dialMargin(10)
		.snap(false);
	
	/* logical private variables */
	this.currentAngle = 0;
	this.s = null;
	this.ltickstart = 0;
	this.ang = 0;
	this.snapVal = 0;
}

wc.extend(wc.Knob, wc.Base);

wc.Knob.prototype
    .property("x")
    .property("y")
    .property("r")
    .property("minVal")
	.property("maxVal")	
	.property("startVal")
	.property("largeTick")
	.property("smallTick")
	.property("minAngle")
	.property("maxAngle")
	.property("dialRadius")
	.property("ltlen") // largeticklength
	.property("stlen") // smallticklength
	.property("dialMargin")
	.property("snap")
	.property("selectOpts");  // expects an array in the format of [["key1", "value1"], ["key2", "value2"], ...]  e.g. [[0, "a"], [1, "b"], [2, "c"], [3, "d"]]

wc.Knob.prototype.create = function (r, x, y) {	
	this.r(r);
	this.x(x);
	this.y(y);
	
	if (this.selectOpts() == undefined) {
		this.drawDial(this.largeTick(), this.ltlen(), true);
		this.drawDial(this.smallTick(), this.stlen(), false);
	}
	else {
		this.minVal(0);
		this.maxVal(this.selectOpts().length - 1);
		this.largeTick(1);
		this.smallTick(1);
		this.snap(true);
		this.drawDial(this.largeTick(), this.ltlen(), true, true);
	}
	
	this.drawKnob();
	
	return this;
}

wc.Knob.prototype.drawDial = function(tick, length, isLargeTick) {
	var maxVal = this.maxVal();
	var minVal = this.minVal();
	var maxAngle = this.maxAngle();
	var minAngle = this.minAngle();
	var cx = this.x();
	var cy = this.y();
	var radius = this.dialRadius();
	var maxValAlt = Math.floor(maxVal / tick) * tick;
	var minValAlt = Math.ceil(minVal / tick) * tick;				
	var n = Math.floor((maxValAlt - minValAlt) / tick);				
	var tickAngle = tick * (maxAngle - minAngle) / (maxVal - minVal);
	var startAngle = 0;
	
	if (minVal >= 0) 
		startAngle = ((minVal % tick) == 0)? 0 : (tick - minVal % tick) * (maxAngle - minAngle) / (maxVal - minVal);
	else 
		startAngle = (-minVal % tick) * (maxAngle - minAngle) / (maxVal - minVal);

	if (isLargeTick) {
		this.ltickstart = minAngle + startAngle;
		this.snapVal = this.ltickstart;
	}
	
	for (var i = 0; i <= n; i++) {
		var ang = (minAngle + startAngle + i * tickAngle);
		this.r().path("M" + cx + " " + (cy + radius) + "L" + cx+ " " + (cy + radius + length)).attr({rotation: ang + " " + cx + " " + cy, "stroke-width": isLargeTick? 2 : 1, stroke: "#fff"});		
		if (isLargeTick) 
		{
			if (this.selectOpts() == undefined) {
				if (ang >= 90 && ang <= 270) {
					if (minValAlt + i * tick == 0)
						this.r().text(cx, cy - radius - 25, "0").attr({rotation:(ang-180) + " " + cx + " " + cy, "stroke-width": 1, stroke: "#fff"});						
					else 
						this.r().text(cx, cy - radius - 25, minValAlt + i * tick).attr({rotation:(ang-180) + " " + cx + " " + cy, "stroke-width": 1, stroke: "#fff"});						
				}
				else {
					if (minValAlt + i * tick == 0)
						this.r().text(cx, cy + radius + 25, "0").attr({rotation:ang + " " + cx + " " + cy, "stroke-width": 1, stroke: "#fff"});						
					else 
						this.r().text(cx, cy + radius + 25, minValAlt + i * tick).attr({rotation:ang + " " + cx + " " + cy, "stroke-width": 1, stroke: "#fff"});						
				}
			}
			else {
				if (Math.round(ang) == 0 || Math.round(ang) == 360) {
					this.r().text(cx, cy + radius + 25, this.selectOpts()[i]).attr({"stroke-width": 1, stroke: "#fff"});
				}
				else if (Math.round(ang) == 180) {
					this.r().text(cx, cy - radius - 25, this.selectOpts()[i]).attr({"stroke-width": 1, stroke: "#fff"});
				}
				else if (ang > 0 && ang < 180) {
					var rad = ang * Math.PI / 180;
					this.r().text(cx - (radius + 25) * Math.sin(rad), cy + (radius + 25) * Math.cos(rad), this.selectOpts()[i]).attr({"stroke-width": 1, stroke: "#fff", "text-anchor":"end"});					
				}
				else {// if (ang > 180)
					var rad = ang * Math.PI / 180;
					this.r().text(cx - (radius + 25) * Math.sin(rad), cy + (radius + 25) * Math.cos(rad), this.selectOpts()[i]).attr({"stroke-width": 1, stroke: "#fff", "text-anchor":"start"});
				}
			}
		}
	}
	
	this.ang = this.largeTick() * (this.maxAngle() - this.minAngle()) / (this.maxVal() - this.minVal());
	
	return this;
}

wc.Knob.prototype.drawKnob = function() {
	var r = this.r(), radius = this.dialRadius(), cx = this.x(), cy = this.y();
	var knobInnerRadius = radius - this.dialMargin();
	r.circle(cx, cy, radius - 5).attr({"stroke-width": 2, stroke: "none", fill: "r(0.5, 0.5)#fff-#333"});
	/*var element = */ r.circle(cx, cy, knobInnerRadius).attr({"stroke-width": 2, stroke: "none", fill: "#777"});
	this.initMark();
	var element = r.circle(cx, cy, radius + this.ltlen()).attr({stroke: "none", fill: "#777", "fill-opacity":0});	
	var len = radius + this.ltlen();
	$(element.node).mousedown(mouseUpAfterDrag);
	
	var that = this;
	
	function mouseUpAfterDrag(e) {			
		/* You can record the starting position with */
		//var start_x = e.pageX;
		//var start_y = e.pageY;
		var offsetE = $(element.node).offset();
		var currentX = e.pageX - offsetE.left;
		var currentY = e.pageY - offsetE.top;
	
		$(element.node).mousemove(function(e) {
			/* And you can get the distance moved by */
			//var offset_x = e.pageX - start_x;
			//var offset_y = e.pageY - start_y;
			
			var nowX = e.pageX - offsetE.left; 
			var nowY = e.pageY - offsetE.top;
			
			var x1 = currentX - len;
			var y1 = len - currentY;
			var x2 = nowX - len;
			var y2 = len - nowY;
			
			var angle = 180 * (Math.atan2(x2, y2) - Math.atan2(x1, y1)) / Math.PI;

			that.setRelativeValue(angle);
			
			currentX = nowX;
			currentY = nowY;
		});
	
		$(element.node).one('mouseup', function() {
			//alert("This will show after mousemove and mouse released.");
			$(element.node).unbind();
			$(element.node).mousedown(mouseUpAfterDrag);
		});
		
		$(element.node).one('mouseleave', function() {
			//alert("This will show after mousemove and mouse released.");
			$(element.node).unbind();
			$(element.node).mousedown(mouseUpAfterDrag);
		});
		
		$(element.node).one('mouseout', function() {
			//alert("This will show after mousemove and mouse released.");
			$(element.node).unbind();
			$(element.node).mousedown(mouseUpAfterDrag);
		});
	
		// Using return false prevents browser's default,
		// often unwanted mousemove actions (drag & drop)
		return false;
	}
}

wc.Knob.prototype.initMark = function () {
	var r = this.r(), radius = this.dialRadius(), cx = this.x(), cy = this.y(), minAngle = this.minAngle();
	this.s = r.set();                
	this.s.push(r.rect(cx - 2, cy + radius - 25, 4, 15, 2).attr({stroke: "none", fill:"#D00"})); 
	this.s.push(r.rect(cx - 2, cy + radius - 11, 4, 5).attr({stroke: "none", fill:"#B00"})); 
	if (this.startVal() == undefined) {
		this.s.animate({rotation:minAngle + " " + cx + " " + cy}, 0, "<>");
		this.currentAngle = minAngle;	
	}
	else {	 
		 this.currentAngle = this.minAngle() + (this.maxAngle() - this.minAngle())* (this.startVal() - this.minVal()) / (this.maxVal() - this.minVal());
		 this.s.attr({rotation:this.currentAngle + " " + cx + " " + cy});
	}
}	

wc.Knob.prototype.setRelativeValue = function (val) {
	if (this.currentAngle + val > this.maxAngle()) 
	{
		this.s.animate({rotation:this.maxAngle() + " " + this.x() + " " + this.y()}, 0, ">");
		this.currentAngle = this.maxAngle();
	}
	else if (this.currentAngle + val < this.minAngle()) 
	{
		this.s.animate({rotation:this.minAngle() + " " + this.x() + " " + this.y()}, 0, ">");
		this.currentAngle = this.minAngle();
	}
	else 
	{
		if (this.snap()) {			
			var a = Math.round(this.ltickstart + this.ang * Math.round((this.currentAngle + val - this.ltickstart) / this.ang));
			this.s.animate({rotation:a + " " + this.x() + " " + this.y()}, 120, ">");
			if (this.snapVal != a)
			{
				var vall = this.minVal() + (a - this.minAngle()) * (this.maxVal() - this.minVal()) / (this.maxAngle() - this.minAngle());
				this.onChange(vall);
			}
			
			this.snapVal = a				
			this.currentAngle += val;
			return;
		}
		else {
			this.s.animate({rotation:(this.currentAngle + val) + " " + this.x() + " " + this.y()}, 0, ">");
			this.currentAngle += val;
		}		
	}
	
	var val = this.minVal() + (this.currentAngle - this.minAngle()) * (this.maxVal() - this.minVal()) / (this.maxAngle() - this.minAngle());
	this.onChange(val);	
}

wc.Knob.prototype.onChange = function (val) {	
}

//## Label 
wc.Label = function() {
	wc.Base.call(this);
    /* @private */
    this.x(0)
        .y(0)
        .r(null)
		.text("Hello")
		.font(undefined)
		.fontfamily("verdana")
		.fontsize(12)
		.letterspacing(20)
		.align("middle")
		.rotation(0)
		.fill("#fff")
		.stroke("#fff");
	/* @private */    
	this.g = null;
};

wc.extend(wc.Label, wc.Base);

wc.Label.prototype
    .property("x")
    .property("y")
    .property("r")
	.property("text")
	.property("font")
	.property("fontfamily")
	.property("fontsize")
	.property("letterspacing")
	.property("align")
	.property("rotation")
	.property("fill")
	.property("stroke");

wc.Label.prototype.create = function (r, x, y) {
	this.r(r);
	this.x(x);
	this.y(y);
	var t = this;
	if (t.font() == undefined)
		this.g = t.r().text(t.x(), t.y(), t.text()).attr({fill:this.fill, stroke:this.stroke, "font-family":t.fontfamily(), "font-size":t.fontsize(), 'letter-spacing':t.letterspacing(), rotation:this.rotation() + " " + this.x() + " " + this.y(), "text-anchor":this.align()});
	else 
		this.g = t.r().text(t.x(), t.y(), t.text()).attr({fill:this.fill, stroke:this.stroke, font: t.font(), "line-spacing":t.letterspacing(), rotation:this.rotation() + " " + this.x() + " " + this.y(), "text-anchor":this.align()});
	return this;
}

wc.Label.prototype.update = function (txt) {
	//this.g.remove();
	this.text(txt);
	var t = this;
	/*if (t.font() == undefined)
		this.g = t.r().text(t.x(), t.y(), t.text()).attr({fill:"none", stroke:"#fff", "font-family":t.fontfamily(), "font-size":t.fontsize(), 'letter-spacing':t.letterspacing()});
	else 
		this.g = t.r().text(t.x(), t.y(), t.text()).attr({fill:"none", stroke:"#fff", font: t.font(), "line-spacing":t.letterspacing()});*/
	this.g.attr({"text":txt});
	
	return this;
}

//## Linear Gauge

wc.LGauge = function() {
	wc.Base.call(this);
    /* @private */
    this.x(50)
        .y(200)
        .r(null)
		.length(300)
		.minVal(0)
		.maxVal(1000)
		.largeTick(100)
		.smallTick(10)
		.needleLength(30)
		.orient("h")
		.stlen(10)
		.ltlen(15);
	/* @private */    
	this.s = null;
	this.currentX = 0;
};

wc.extend(wc.LGauge, wc.Base);

wc.LGauge.prototype
    .property("x")
    .property("y")
    .property("r")
	.property("length")
	.property("minVal")
	.property("maxVal")
	.property("largeTick")
	.property("smallTick")
	.property("needleLength")
	.property("orient")
	.property("stlen")
	.property("ltlen");
	

wc.LGauge.prototype.create = function (r, x, y) {
	this.r(r);
	this.x(x);
	this.y(y);
	this.drawDial(this.largeTick(), this.ltlen(), true);
	this.drawDial(this.smallTick(), this.stlen(), false);			
	this.initNeedle();	
	this.currentX = x;
	return this;
}

wc.LGauge.prototype.drawDial = function (tick, tickLength, isLargeTick) {
	var r = this.r(), x = this.x(), y = this.y(), length = this.length(), maxVal = this.maxVal(), minVal = this.minVal();
	var maxValAlt = Math.floor(maxVal / tick) * tick;
	var minValAlt = Math.ceil(minVal / tick) * tick;				
	var n = Math.floor((maxValAlt - minValAlt) / tick);				
	var tickC = tick * (length) / (maxVal - minVal);
	var startC = 0;
	if (minVal >= 0) 
		startC = ((minVal % tick) == 0)? 0 : (tick - minVal % tick) * (length) / (maxVal - minVal);
	else 
		startC = (-minVal % tick) * (length) / (maxVal - minVal);
	for (var i = 0; i <= n; i++) {
		var locX = (x + startC + i * tickC);
		r.path("M" + locX + " " + y + "L" + locX + " " + (y - tickLength)).attr({"stroke-width": isLargeTick? 2 : 1, stroke: "#aaa"});		
		if (isLargeTick) 
		{
			if (minValAlt + i * tick == 0)
				r.text(locX, y - tickLength - 5, "0").attr({"stroke-width": 1, stroke: "#aaa"});
			else
				r.text (locX, y - tickLength - 5, minValAlt + i * tick).attr({"stroke-width": 1, stroke: "#aaa"});
		}
	}
	r.path("M" + x + " " + y + "L" + (x + length) + " " + y).attr({stroke: "#fff"});
}

wc.LGauge.prototype.initNeedle = function() {
	var needleLength = this.needleLength(), x = this.x(), y = this.y(), length = this.length(), r = this.r();
	this.s = r.set();                
	this.s.push(r.path("M" + x + " " + (y - 25) + " L" + x + " " + (y + needleLength - 25)).attr({fill: "none", "stroke-width": 3, stroke: "#f00"}));
}						

wc.LGauge.prototype.setValue = function (val) {
	var minVal = this.minVal(), maxVal = this.maxVal(), x = this.x(), length = this.length();
	var valCalculated = (val - minVal) * length / (maxVal - minVal) + x;
	this.s.animate({translation :(valCalculated - this.currentX) + " 0"}, Math.abs((valCalculated - this.currentX) * 5), "<>");
	this.currentX = valCalculated;
};


//## Circular Gauge

wc.CGauge = function() {
	wc.Base.call(this);
    /* @private */
    this.x(50)
        .y(200)
        .r(null)
		.dialRadius(60)
		.minVal(0)
		.maxVal(1000)
		.minAngle(30)
		.maxAngle(330)
		.largeTick(100)
		.smallTick(10)
		.stlen(10)
		.ltlen(15)
		.needleCenterRadius(5)
		.labelOffset(10)
		.tickcolor("#fff")
		.needlecolor("#aaa");
	/* @private */    
	this.s = null;
	this.currentX = 0;
};

wc.extend(wc.CGauge, wc.Base);

wc.CGauge.prototype
    .property("x")
    .property("y")
    .property("r")
	.property("minVal")
	.property("maxVal")
	.property("largeTick")
	.property("smallTick")
	.property("needleLength")
	.property("needleBottom")
	.property("needleCenterRadius")
	.property("dialRadius")
	.property("minAngle")
	.property("maxAngle")
	.property("stlen")
	.property("ltlen")
	.property("labelOffset")
	.property("labelFontSize")
	.property("tickcolor")
	.property("needlecolor");

wc.CGauge.prototype.create = function (r, x, y) {
	this.r(r);
	this.x(x);
	this.y(y);
	this.drawDial(this.largeTick(), this.ltlen(), true);
	this.drawDial(this.smallTick(), this.stlen(), false);			
	this.initNeedle();	
	return this;
}

wc.CGauge.prototype.drawDial = function (tick, length, isLargeTick) {
	var r = this.r(), radius = this.dialRadius(), cx = this.x(), cy = this.y(), minVal = this.minVal(), maxVal = this.maxVal(), minAngle = this.minAngle(), maxAngle = this.maxAngle(), tickcolor = this.tickcolor(), needlecolor = this.needlecolor();
	var maxValAlt = Math.floor(maxVal / tick) * tick;
	var minValAlt = Math.ceil(minVal / tick) * tick;				
	var n = Math.floor((maxValAlt - minValAlt) / tick);				
	var tickAngle = tick * (maxAngle - minAngle) / (maxVal - minVal);
	var startAngle = 0;
	if (minVal >= 0) 
		startAngle = ((minVal % tick) == 0)? 0 : (tick - minVal % tick) * (maxAngle - minAngle) / (maxVal - minVal);
	else 
		startAngle = (-minVal % tick) * (maxAngle - minAngle) / (maxVal - minVal);
	for (var i = 0; i <= n; i++) {
		var ang = (minAngle + startAngle + i * tickAngle);
		r.path("M" + cx + " " + (cy + radius) + "L" + cx+ " " + (cy + radius - length)).attr({rotation: ang + " " + cx + " " + cy, "stroke-width": isLargeTick? 2 : 1, stroke: tickcolor});		
		if (isLargeTick) 
		{
			if (this.labelFontSize() == undefined) 
				this.labelFontSize(10);
			if (ang >= 90 && ang <= 270) {
				if (minValAlt + i * tick == 0)
					r.text(cx, cy - radius - this.labelOffset(), "0").attr({rotation:(ang-180) + " " + cx + " " + cy, "stroke-width": 1, stroke: tickcolor, "font-size":this.labelFontSize(), fill: tickcolor});						
				else 
					r.text(cx, cy - radius - this.labelOffset(), minValAlt + i * tick).attr({rotation:(ang-180) + " " + cx + " " + cy, "stroke-width": 1, stroke: tickcolor, "font-size":this.labelFontSize(), fill: tickcolor});						
			}
			else {
				if (minValAlt + i * tick == 0)
					r.text(cx, cy + radius + this.labelOffset(), "0").attr({rotation:ang + " " + cx + " " + cy, "stroke-width": 1, stroke: tickcolor, "font-size":this.labelFontSize(), fill: tickcolor});						
				else 
					r.text(cx, cy + radius + this.labelOffset(), minValAlt + i * tick).attr({rotation:ang + " " + cx + " " + cy, "stroke-width": 1, stroke: tickcolor, "font-size":this.labelFontSize(), fill: tickcolor});						
			}
		}
	}
}

wc.CGauge.prototype.initNeedle = function() {
	var cx = this.x(), cy = this.y(), r = this.r(), radius = this.dialRadius(), minAngle = this.minAngle(), needlecolor = this.needlecolor;
	this.s = r.set();                
	if (this.needleBottom() == undefined)
		this.needleBottom(15);		
	if (this.needleLength() == undefined)
		this.needleLength(radius - 5);	
	this.s.push(r.path("M" + cx + " " + (cy - this.needleBottom()) + " L" + cx + " " + (cy + this.needleLength())).attr({fill: "none", "stroke-width": 4, stroke: needlecolor}));
	this.s.push(r.circle(cx, cy, this.needleCenterRadius()).attr({fill: "#aaa", "stroke-width": 10, stroke: "#aaa"}));
	this.s.animate({rotation:minAngle + " " + cx + " " + cy}, 0, "<>");
}						

wc.CGauge.prototype.setValue = function (val) {
	var valCalculated = (val - this.minVal()) * (this.maxAngle() - this.minAngle()) / (this.maxVal() - this.minVal()) + this.minAngle();
	this.s.animate({rotation:valCalculated + " " + this.x() + " " + this.y()}, 800, ">");
};


//## Seven Segment Display

wc.SSegArray = function() {
	wc.Base.call(this);
    /* @private */
    this.x(900)
        .y(240)
        .r(null)
		.count(6)
		.decimal(2)
		.gap(130)
		.scale(1)
		.coloroff("#01232D")
		.coloron("#00FFFF")
		.initialValue(0);
		
	/* @private */    
	this.s = null;
	this.digits = [];
};

wc.extend(wc.SSegArray, wc.Base);

wc.SSegArray.prototype
    .property("x")
    .property("y")
    .property("r")
	.property("count")
	.property("decimal")
	.property("gap")
	.property("scale")
	.property("coloroff")
	.property("coloron")
	.property("initialValue");

wc.SSegArray.prototype.create = function (r, x, y) {
	function drawSegment(r, cx, cy, l, a, b, c, angle, color) {
		return r.path("M" + (cx + l) + " " + cy + "L" + (cx + l - a + c) + " " + (cy - b) + "L" + (cx - l + a + c) + " " + (cy - b) + "L" + (cx - l) + " " + (cy) + "L" + (cx - l + a - c) + " " + (cy + b) + "L" + (cx + l - a - c) + " " + (cy + b)).attr({fill:color, rotation:angle, stroke:"none"}); 
	}
	
	function drawDigit(r, cx, cy, scale, color) {
		var ll = 40 * scale;
		var aa = 10 * scale;
		var bb = 10 * scale;
		var cc = 2 * scale;
		var rr = 7 * scale;
		var a = drawSegment(r, cx, cy, ll, aa, bb, cc, 0, color);  
		var g = drawSegment(r, cx - 14 * scale, cy + 84 * scale, ll, aa, bb, cc, 0, color);
		var d = drawSegment(r, cx - 28 * scale, cy + 168 * scale, ll, aa, bb, cc, 0, color);
		
		var b = drawSegment(r, cx + 35 * scale, cy + 42 * scale, ll, aa, bb, -cc, 100, color);
		var c = drawSegment(r, cx + 21 * scale, cy + 126 * scale, ll, aa, bb, -cc, 100, color);
		var f = drawSegment(r, cx - 48 * scale, cy + 42 * scale, ll, aa, bb, -cc, 100, color);
		var e = drawSegment(r, cx - 62 * scale, cy + 126 * scale, ll, aa, bb, -cc, 100, color);
		var dot = r.circle(cx + 32 * scale, cy + 175 * scale, rr).attr({fill:color, stroke:"none"}); ; 
		return [a, b, c, d, e, f, g, dot]; 
	}
	
	this.r(r);
	this.x(x);
	this.y(y);
	
	for (var i = 0; i < this.count(); i++) {					
		this.digits[i] = drawDigit(this.r(), this.x() - this.gap() * this.scale() * i, this.y(), this.scale(), this.coloroff()); 
	}
	
	this.setValue(this.initialValue());
	
	return this;
}

wc.SSegArray.prototype.setValue = function (val) {
	function litDigit(digit, val, dot, coloron, coloroff) {
		var cond = [];
		switch (val) {
			case 1:
				cond = [0, 1, 1, 0, 0, 0, 0];
				break;
			case 2:
				cond = [1, 1, 0, 1, 1, 0, 1];
				break;
			case 3:
				cond = [1, 1, 1, 1, 0, 0, 1];
				break;
			case 4:
				cond = [0, 1, 1, 0, 0, 1, 1];
				break;
			case 5:
				cond = [1, 0, 1, 1, 0, 1, 1];
				break;
			case 6:
				cond = [1, 0, 1, 1, 1, 1, 1];
				break;
			case 7:
				cond = [1, 1, 1, 0, 0, 0, 0];
				break;
			case 8:
				cond = [1, 1, 1, 1, 1, 1, 1];
				break;
			case 9:
				cond = [1, 1, 1, 1, 0, 1, 1];
				break;
			case 0:
				cond = [1, 1, 1, 1, 1, 1, 0];
				break;					
		}
		
		for (var i = 0; i < 7; i++) {
			if (cond[i] == 1) {
				digit[i].attr({fill:coloron});
			}
			else {
				digit[i].attr({fill:coloroff});
			}
		}
		
		if (dot) {
			digit[7].attr({fill:coloron});
		}
		else {
			digit[7].attr({fill:coloroff});
		}
	}
	
	var value = val * Math.pow(10, this.decimal());
	value = Math.round(value);
	for (var i = 0; i < this.count(); i++) {
		litDigit(this.digits[i], value % 10, (i == this.decimal()), this.coloron(), this.coloroff());
		if (value < 10) 
			break;
		value = Math.floor(value / 10);
	}
}

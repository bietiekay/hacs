#region Usings

using System;

#endregion

namespace Framework.Serialization
{
	/// <summary>                                                                                                                                                                                                         
	/// This interface has to be implemented by any object that should                                                                                                                                                    
	/// be serialized by the new FastSerializer.                                                                                                                                                                          
	/// </summary>                                                                                                                                                                                                        
	public interface IFastSerialize                                                                                                                                                                                       
	{                                                                                                                                                                                                                     
		/// <summary>                                                                                                                                                                                                     
		/// This method serializes the implementing object                                                                                                                                                                
		/// </summary>                                                                                                                                                                                                    
		/// <returns>the serialized data as byte array</returns>                                                                                                                                                          
		byte[] Serialize();                                                                                                                                                                                               

		/// <summary>                                                                                                                                                                                                     
		/// This method deserializes the given data into the object that implements this interface                                                                                                                        
		/// </summary>                                                                                                                                                                                                    
		void Deserialize(byte[] Data);                                                                                                                                                                                    

	}         
}
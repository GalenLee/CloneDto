# Multiple Ways To Clone A DTO
How many ways can you think of to clone a simple DTO?

This project shows multiple ways to clone a simple DTO.  Some of the ways shown will do a deep copy.  

- Implementing ICloneable (must be part of the DTO)
- MemberwiseClone (protected method)
- Serialize - Binary (deep copy)
- Serialize - XML Serializer (deep copy)
- Compiled Lambda Expression Tree (this is fast)
- Reflection (deep copy)
- Dynamically Generated IL (not done)
# Multiple Ways To Clone A DTO
How many ways can you think of to clone a simple DTO?

This project shows multiple ways to clone a simple DTO.  Most of the ways shown have been coded 
for a shallow copy of a DTO with simple types.  However, most of the approaches could be expanded
to perform a deep copy on more complex DTOs.

- Implementing ICloneable
- MemberwiseClone (protected method)
- Serialize - Binary
- Serialize - XML Serializer
- Compile Lambda Expression Tree
- Reflection
- Dynamically Generated IL (not done)
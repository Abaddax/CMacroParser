# C Macro Parser
A basic C-Macro parser written in C#.

# Suppored Features
* Basic/Const defines
   - `#define PI 3.1415`
* Function like macros 
   - `#define POW2(x) x * x`
* Macro expansion
   - `#define MULT(x,y) x * y`
   - `#define DOUBLE(x) MULT(x, 2)`
   - `MULT2(2)` -> `2 * 2`
* Basic type deduction
   - `#define INT(x) (int)x` -> `int`
   - `#define PI 3.1415` -> `double` 
   - `#define PI 3.1415f` -> `float`
   - `#define GT(x,y) x > y` -> `bool`

The parser transforms the macro into a C# representation, but does not support any meaningfull calcucations/functuions with the parsed macros.

This parser is not feature complete and does not aim to be and and bugs might happen.
Also more complex macros with inline struct initialization, templates, variadic macros, ... are not supported.

The main purpose of this project is to parse const value macros like `S_OK` for [BindingsGenerator](https://github.com/Abaddax/BindingsGenerator) 
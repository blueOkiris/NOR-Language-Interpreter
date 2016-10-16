# NOR-Language-Interpreter

## Interpreter
This is a simple switch-case interpreter I made to create an interpreter for my programming language, NOR.
It is an esoteric programming language that is purposely tedious to program in.
Check out https://esolangs.org/wiki/NOR 

## What is NOR
A programming language with 9 commands, but only one important command: NOR.
When you program this you think in terms of a circuit, and the only data is line numbers.

### Data Manipulation
NOR stores data in a strange way. Instead of a data pointer or variables, every time you call the NOR operation, it stores that operation as that line's value. This can be accessed by other NOR calls and outputs. You can access these stored values for lines by placing a "#" followed by a number, a ":", and a default value ("0" or "1"). When the interpreter reads this value, it does the default value if the number hasn't happened yet. It also as an array of input values for program input. At the beginning of each program, you place a line of a single number which is the size of that array. You access the values of this array by saying "IN" and then the index of that array.

Example to show Data Manipulation:

"100 NOR #300:0, 1" - stores the NOR of line 300 and 1 into line 100. Except that at the start, line 300 hasn't happened yet, so it does the default value of 0.

"200 OUT #100:1" - prints the value of that last NOR

"300 NOR 0, IN0" - stores the NOR of 0 and input[0]

"400 JMP 100" - Jumps back to 100, now with a value for line 300

## Op Codes
NOR has 9 OpCodes: NOR, INP, OUT, OLN, JMP, REM, and RND, and it works much like basic, with line numbers.
Commas are optional, but they help to make the code look nice.

#### NOR
    LN NOR #1, #2
    At line LN, set the line value to the NOR operation of #1 and #2
#### INP	
    LN INP IN# 
    At line LN, input a 0 or 1 to the array of input values
#### OUT
    LN OUT # 
    At line LN, prints out the value of #
#### OLN	
    LN OLN 
    At line LN, prints a new line
#### JMP	
    LN JMP # 
    At line LN, jump to line #
#### REM	
    LN REM bah blah 
    Works as a comment (NOTE: still needs a line number)
#### RND	
    LN RND IN# 
    At line LN, sets input[#] to a random value between 0 and 1, or without argument sets the line to a random number.
#### OFF	
    LN OFF 
    At Line LN, turn off the circuit (end the program)
#### MUX	
    LN MUX #, J1, J2 
    At line LN, if # is 0 go to line J1, and if # is 0 go to line J2. This is essentially a multiplexer.

Obviously the key opcode is the NOR opcode. Not only does it work as a NOR, but it can be used to represent some conditionals

## Example programs
Example programs can be found in the bin/Debug/ folder

# LISP interpreter in C#

## Build

Install .NET (`sudo apt-get install dotnet-sdk-8.0`).
Run the build command in the project root
```
dotnet build
```

## Usage

```
./Lisp <program.lisp>
```


## Running the demos

```
cd Lisp/bin/Debug/net8.0
./Lisp examples/demo.lisp
./Lisp examples/chleba.lisp
```

See https://youtu.be/95JR6VJh8ho

## Features
```lisp
﻿; Primitive data types: numbers, strings, bools, nil
(print-ln "=== Primitive data types ===")
(print-object-ln 123)
(print-ln "asdf")
(print-object-ln #t)
(print-object-ln nil)

; Calling functions on objects
(print-ln "=== Function calling ===")
(print-object-ln (+ 123 456))
(print-ln (if (> (+ 1 1) (- 3 2))
                     "1+1 is greater than 3-2"
                     "1+1 is not greater than 3-2"))
(print-object-ln (+ 1 2 3 4 5 6))
(print-object-ln (+))

; Combine primitve data types into lists
(print-ln "=== Lists ===")
(print-object-ln (list 1 2 3))
(print-object-ln (list 1 (list #f (list "str"))))

; Create an expression fromi primitve data types and evaluate it
(print-ln "=== eval function ===")
(define my-variable 69)
(define expression-condition (> 0 1))
(define my-expression (list '+ 'my-variable 420 (list (list 'if expression-condition '* '+) 666 0 )))

(print "      my-expression : ") (print-object-ln my-expression)
(print "(eval my-expression): ") (print-object-ln (eval my-expression))

; Define your own functions using define and lambda
(print-ln "=== User defined functions ===")
(define factorial (lambda (n)
    (if (= n 0)
        1
        (* n (factorial (- n 1))))))
(print-object-ln (factorial 5))

(define func+1 (lambda (n) (+ n 1)))
(define func/2 (lambda (n) (/ n 2)))
(define my-composed-func (compose func/2 func+1))
(print-object-ln (my-composed-func 3))

; Use lambda functions to work with lists
(print-ln "=== List processing ===")
(define my-list (list 1 2 3 4 5 6 7))
(print-object-ln (length my-list))
(print-object-ln (append my-list (list 8 9)))

(print-object-ln (map (lambda (n) (* n 2)) my-list))
(print-object-ln (filter even my-list))
```

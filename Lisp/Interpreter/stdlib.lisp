(define list (lambda items items))

(define list? (lambda (xs)
    (if (nil? xs) #t (and (pair? xs) (list? (cdr xs))))))

(define length (lambda (xs)
    (cond ((nil? xs) 0) 
          ((pair? xs) ( + 1 (length (cdr xs)))))))

(define foldr (lambda (f e xs)
    (cond ((nil? xs) e)
          ((pair? xs) (f (car xs) (foldr f e (cdr xs)))))))

(define append2 (lambda (xs ys)
    (foldr cons ys xs)))

(define append (lambda xss (foldr append2 nil xss)))

(define curry2 (lambda (f)
    (lambda (x) (lambda (y) (f x y)))))

(define uncurry2 (lambda (f)
    (lambda (x y) ((f x) y))))

(define compose2 (lambda (f g)
    (lambda (x) (f (g x)))))

(define id (lambda (x) x))

(define compose (lambda fs (foldr compose2 id fs)))

(define map (lambda (f xs)
    (foldr (uncurry2 (compose (curry2 cons) f)) nil xs)))

(define append-map (lambda (f xs)
    (apply append (map f xs))))

(define filter (lambda (f xs)
    (cond ((nil? xs) nil)
          ((pair? xs) (if (f (car xs))
                          (cons (car xs) (filter f (cdr xs)))
                          (filter f (cdr xs)))))))

(define list-ref (lambda (xs n)
    (if (= n 0)
        (car xs)
        (list-ref (cdr xs) (- n 1)))))

(define make-list (lambda (f n)
    (let ((iter (lambda (i acc)
                    (if (< i 0)
                        acc
                        (iter (- i 1) (cons (f i) acc))))))
         (iter (- n 1) nil))))

(define for-each (lambda (f xs)
    (if (pair? xs) 
        (begin (f (car xs)) (for-each f (cdr xs))))))

(define print (lambda (string)
    (for-each print-char string)))

(define print-ln (lambda (string)
    (print string)
    (print "\n")))

(define print-object (compose print to-string))

(define print-object-ln (lambda (x)
    (print-object x)
    (print "\n")))

(define not (lambda (x)
    (if x #f #t)))

(define even (lambda (n)
    (= (% n 2) 0)))

(define odd (compose not even))

(define b+ +)
(define + (lambda xs (foldr b+ 0 xs)))
(define b* *)
(define * (lambda xs (foldr b* 1 xs)))

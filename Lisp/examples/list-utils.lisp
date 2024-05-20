(define take (lambda (xs n)
    (if (= n 0)
        nil
        (cons (car xs) (take (cdr xs) (- n 1))))))

(require "list-utils.lisp")

(define pečiva-1.pád (list "chleba" "rohlík" "houska" ))
(define pečiva-5.pád (list "chlebe" "rohlíku" "housko" ))
(define úrovně (list "" " s máslem" " s máslem, se salámem"))

(define vytvoř-postavy (lambda (pečiva úrovně)
    (append-map (lambda (p) (map (lambda (u) (append p u)) úrovně)) pečiva)))

(define spoj-pečiva (lambda (pečiva)
    (let ((délka (length pečiva)))
         (if (= délka 1)
             (car pečiva)
             (append (append-map (lambda (x) (append x ", ")) (take pečiva (- délka 2))) (list-ref pečiva (- délka 2)) " a " (list-ref pečiva (- délka 1)))))))

(define iterace-jednotné (lambda (nový starý-1 starý-5 odpověď) (append "Jde " starý-1 " a potká " nový ". A " nový " povídá: " starý-5 ", můžu jít s tebou? Přičemž " starý-1 " odpoví: " odpověď)))
(define iterace-množné (lambda (nový staří-1 staří-5 odpověď) (append "Jde " staří-1 " a potkají " nový ". A " nový " povídá: " staří-5 ", můžu jít s váma? Přičemž " staří-1 " odpoví: " odpověď)))
(define iterace (lambda (nový staří-1 staří-5 odpověď)
    ((if (= (length staří-1) 1) iterace-jednotné iterace-množné) nový (spoj-pečiva staří-1) (spoj-pečiva staří-5) odpověď)))

(define příběh
    (let ((rajče "rajče")
          (postavy-1.pád (vytvoř-postavy pečiva-1.pád úrovně))
          (postavy-5.pád (vytvoř-postavy pečiva-5.pád úrovně))
          (délka (length postavy-1.pád))
          (odpověď-jo "Jo, můžeš. ")
          (odpověď-ne "Ne. (čas pro smích)")
          (úvod (append-map (lambda (n) (iterace (list-ref postavy-1.pád n) (take postavy-1.pád n) (take postavy-5.pád n) odpověď-jo)) (make-list (lambda (x) (+ x 1)) (- délka 1))))
          (závěr (iterace rajče postavy-1.pád postavy-5.pád odpověď-ne)))
         (append úvod závěr)))

(print-native příběh)

# Introduzione API iLTrovaLibro

Benvenuto nella documentazione tecnica del sistema **iLTrovaLibro**. 
Queste API permettono la gestione completa del catalogo libri e delle categorie.

## Autenticazione
Tutte le richieste (eccetto la ricerca pubblica) richiedono un token **JWT** nell'header della richiesta:

`Authorization: Bearer <il_tuo_token>`

## Formato Risposte
Le API rispondono in formato **JSON**. In caso di errore, il corpo della risposta seguirà lo standard:
* `title`: Breve descrizione dell'errore.
* `status`: Codice HTTP.
* `detail`: Dettaglio tecnico dell'eccezione (solo in ambiente di test).
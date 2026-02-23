# 🚀 QUICK START GUIDE

## Brzi start u 5 koraka:

### 1. Otvorite projekat
```
Dvostruki klik na: LibraryManagementSystem.sln
```

### 2. Kreirajte bazu
Otvorite Package Manager Console u Visual Studio i izvršite:
```powershell
cd src/LibraryManagement.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../LibraryManagement.Web
dotnet ef database update --startup-project ../LibraryManagement.Web
```

### 3. Inicijalizujte uloge
Otvorite SQL Server Object Explorer u Visual Studio:
- Pronađite bazu `LibraryManagementDb`
- Kliknite desni klik → New Query
- Kopirajte i izvršite sadržaj iz `InitializeDatabase.sql`

### 4. Pokrenite aplikaciju
```
F5 ili Ctrl+F5
```

### 5. Testirajte
**Opcija A - MVC interfejs:**
- Otiđite na `https://localhost:5001`
- Registrujte se
- Pregledajte knjige

**Opcija B - API (Swagger):**
- Otiđite na `https://localhost:5001/swagger`
- POST /api/AccountApi/register
- POST /api/AccountApi/login
- Authorize sa tokenom
- Testirajte endpoint-e

---

## Testiranje složenog slučaja korišćenja:

### Preko MVC-a:
1. Registrujte 2 korisnika (koristeći različite browsere ili incognito)
2. Dodajte knjigu sa samo 1 primerkom (potrebna Librarian uloga)
3. Prvi korisnik pozajmi knjigu
4. Drugi korisnik rezerviše istu knjigu → dobija poziciju 1 u redu
5. Prvi korisnik vrati knjigu
6. **REZULTAT:** Drugi korisnik automatski dobija status "Ready" na rezervaciji

### Preko API-ja (Swagger):
1. POST /api/AccountApi/register (korisnik 1)
2. POST /api/AccountApi/login → dobijte token1
3. POST /api/BooksApi (dodajte knjigu)
4. POST /api/BooksApi/borrow (pozajmite knjigu)
5. Registrujte korisnika 2, login → token2
6. POST /api/ReservationsApi (rezervišite knjigu) → queue position = 1
7. Vratite se na token1
8. POST /api/BooksApi/return (vratite knjigu)
9. GET /api/ReservationsApi/user/{userId2} → status = Ready!

---

## Dodavanje Librarian uloge:

Nakon registracije, izvršite u bazi:

```sql
-- Pronađite UserId
SELECT Id, Email FROM AspNetUsers WHERE Email = 'vas@email.com';

-- Pronađite RoleId za Librarian
SELECT Id FROM AspNetRoles WHERE Name = 'Librarian';

-- Dodajte ulogu
INSERT INTO AspNetUserRoles (UserId, RoleId) 
VALUES ('copy-user-id-here', 'copy-role-id-here');
```

---

## Česti problemi:

### Problem: Migration ne radi
**Rešenje:**
```powershell
# Obrišite Migrations folder
# Pa ponovo:
dotnet ef migrations add InitialCreate --startup-project ../LibraryManagement.Web
```

### Problem: Connection string greška
**Rešenje:**
- Otvorite `appsettings.json`
- Proverite connection string
- Za LocalDB: `Server=(localdb)\\mssqllocaldb;...`
- Za SQL Server: `Server=.;...` ili `Server=localhost;...`

### Problem: Uloge ne postoje
**Rešenje:**
- Izvršite `InitializeDatabase.sql` skript

### Problem: Ne mogu dodati knjigu
**Rešenje:**
- Potrebna je Librarian ili Admin uloga
- Dodajte ulogu preko SQL-a (gore)

---

## Pomoć:

Za dodatna pitanja ili probleme, pogledajte `README.md` za detaljniju dokumentaciju.

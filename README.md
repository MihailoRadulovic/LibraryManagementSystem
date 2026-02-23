# Sistem za upravljanje bibliotekom

**Student:** Radulović Mihailo, 2022/0542  
**Projektni zadatak** za kurs Objektno-orijentisane tehnologije

---

## 📋 Opis projekta

Kompletan sistem za upravljanje bibliotekom koji omogućava:
- Pretragu i pregled kataloga knjiga
- Pozajmljivanje i vraćanje knjiga sa automatskim obračunom kazni
- Rezervacije knjiga sa redom čekanja (SLOŽEN slučaj korišćenja)
- Upravljanje korisničkim nalozima (čitaoci, bibliotekari, administratori)
- Administraciju knjižnog fonda

---

## 🏗️ Arhitektura

Projekat koristi **slojevitu (layered) arhitekturu** sa sledećim slojevima:

### 1. **Domain Layer** (`LibraryManagement.Domain`)
- Sadrži core entitete (Book, BookCopy, Loan, Reservation, ApplicationUser)
- Enumeracije (BookCopyStatus, LoanStatus, ReservationStatus)
- Interfejse za repozitorijume (IRepository<T>, IUnitOfWork)
- **Nezavisan od eksterne infrastrukture**

### 2. **Application Layer** (`LibraryManagement.Application`)
- **CQRS pattern** implementiran preko **MediatR** biblioteke
- **Commands** (BorrowBook, ReturnBook, CreateBook, CreateReservation, CancelReservation)
- **Queries** (GetAllBooks, GetBookById, GetUserLoans, GetReservationQueue, GetUserReservations)
- **Handlers** za svaki command i query
- **DTOs** za transfer podataka
- **FluentValidation** za validaciju ulaznih podataka

### 3. **Infrastructure Layer** (`LibraryManagement.Infrastructure`)
- **Entity Framework Core** sa SQL Server
- **ApplicationDbContext** sa Identity integracijom
- **Repository pattern** implementacija
- **Unit of Work** pattern za transakcije
- Konfigurac ija baze podataka i relacija

### 4. **Web Layer** (`LibraryManagement.Web`)
- **ASP.NET Core MVC** za korisnički interfejs
- **Web API kontroleri** za programski pristup (REST API)
- **JWT autentifikacija** za API
- **ASP.NET Core Identity** za MVC autentifikaciju/autorizaciju
- **Swagger/OpenAPI** dokumentacija
- Razor Views za prikaz podataka

---

## 🎯 Slučajevi korišćenja

### Osnovni (5+):
1. **Registracija i autentifikacija korisnika**
2. **Pretraga knjiga** (filteri: naslov, autor, žanr, dostupnost)
3. **Izdavanje knjige** (pozajmljivanje)
4. **Vraćanje knjige**
5. **Kreiranje rezervacije**
6. **Otkazivanje rezervacije**
7. **Administracija knjižnog fonda** (dodavanje knjiga)

### SLOŽEN slučaj korišćenja:
**Rezervacija knjige sa redom čekanja**

Kompleksnost ovog slučaja proizilazi iz:
- **Red čekanja (FIFO):** Kada nema dostupnih primeraka, rezervacije se smeštaju u red po redosledu prijave
- **Automatsko dodeljivanje:** Pri vraćanju knjige, sistem automatski dodeljuje primerak prvom u redu
- **Vremensko ograničenje:** Korisnik ima 3 dana da preuzme knjigu kada postane dostupna
- **Dinamičko ažuriranje pozicija:** Otkazivanje rezervacije automatski pom era pozicije ostalih u redu
- **Procena dostupnosti:** Sistem računa procenjeno vreme dostupnosti na osnovu broja primeraka i pozicije u redu
- **Više entiteta i biznis pravila:** Uključuje Book, Reservation, BookCopy, User sa provera ma blokade, kazni, itd.

**Implementacija:** `CreateReservationCommandHandler.cs` i `ReturnBookCommandHandler.cs`

---

## 🔧 Tehnologije i biblioteke

### Osnovne:
- **ASP.NET Core 8.0** (Web MVC + Web API)
- **Entity Framework Core 8.0** (SQL Server)
- **ASP.NET Core Identity** (autentifikacija/autorizacija)

### Dodatne biblioteke (zahtev za projekat):
- **MediatR 12.2.0** - CQRS pattern implementacija
- **FluentValidation 11.9.0** - validacija komandi i upita
- **AutoMapper 12.0.1** - mapiranje između entiteta i DTO-a
- **JWT Bearer Authentication** - token-based autentifikacija za API
- **Swashbuckle (Swagger) 6.5.0** - OpenAPI dokumentacija

---

## 📦 Pokretanje projekta

### Preduslovi:
- .NET 8.0 SDK
- SQL Server (LocalDB ili puna instanca)
- Visual Studio 2022 ili Visual Studio Code

### Koraci:

1. **Otvorite solution:**
   ```
   Dvostruki klik na LibraryManagementSystem.sln
   ```

2. **Podesite connection string:**
   - Otvorite `LibraryManagement.Web/appsettings.json`
   - Ako koristite LocalDB, connection string je već podešen
   - Za SQL Server instancu, izmenite `ConnectionStrings:DefaultConnection`

3. **Kreirajte bazu podataka:**
   ```bash
   # U Package Manager Console ili terminalu:
   cd src/LibraryManagement.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../LibraryManagement.Web
   dotnet ef database update --startup-project ../LibraryManagement.Web
   ```

4. **Inicijalizacija uloga (VAŽNO):**
   Nakon pokretanja aplikacije, potrebno je ručno dodati uloge u bazu:
   
   ```sql
   -- Pokrenite u SQL Server Management Studio ili preko Visual Studio (SQL Server Object Explorer)
   USE LibraryManagementDb;
   
   INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
   VALUES 
   (NEWID(), 'Admin', 'ADMIN', NEWID()),
   (NEWID(), 'Librarian', 'LIBRARIAN', NEWID()),
   (NEWID(), 'Reader', 'READER', NEWID());
   ```

5. **Pokrenite aplikaciju:**
   ```bash
   cd src/LibraryManagement.Web
   dotnet run
   ```

6. **Pristupite aplikaciji:**
   - **MVC interfejs:** `https://localhost:5001`
   - **Swagger API:** `https://localhost:5001/swagger`

---

## 🧪 Testiranje sistema

### Pristup preko MVC-a:

1. **Registrujte se:**
   - Home → Registruj se
   - Popunite formu (po default-u dobijate ulogu "Reader")

2. **Pretražite knjige:**
   - Knjige → pregledajte katalog
   - Koristite filtere za pretragu

3. **Pozajmite knjigu:**
   - Knjige → Detalji → Pozajmi knjigu
   - Proverite: Moje pozajmice

4. **Rezervišite knjigu:**
   - Kada nema dostupnih primeraka
   - Knjige → Detalji → Rezerviši knjigu
   - Proverite: Moje rezervacije

5. **Vratite knjigu:**
   - Moje pozajmice → Vrati
   - Sistem automatski računa kazne za kašnjenje (50 RSD po danu)

### Pristup preko API-ja (Swagger):

1. **Otvorite Swagger UI:** `https://localhost:5001/swagger`

2. **Registrujte korisnika:**
   ```
   POST /api/AccountApi/register
   ```

3. **Login i dobijte JWT token:**
   ```
   POST /api/AccountApi/login
   ```
   - Kopirajte token iz odgovora

4. **Autorizujte se u Swagger-u:**
   - Kliknite "Authorize" dugme (gornji desni ugao)
   - Unesite: `Bearer {token}`

5. **Testirajte endpoint-e:**
   ```
   GET /api/BooksApi - pregled knjiga
   POST /api/BooksApi/borrow - pozajmljivanje
   POST /api/ReservationsApi - kreiranje rezervacije
   GET /api/ReservationsApi/queue/{bookId} - red čekanja
   ```

---

## 👥 Uloge korisnika

### Reader (Čitalac)
- Pregled kataloga knjiga
- Pozajmljivanje i vraćanje knjiga
- Kreiranje i otkazivanje rezervacija
- Pregled sopstvenih pozajmica i rezervacija

### Librarian (Bibliotekar)
- Sve što i Reader
- Dodavanje novih knjiga
- Upravljanje knjižnim fondom

### Admin (Administrator)
- Sve što i Librarian
- Blokiranje korisnika
- Upravljanje ulogama

**Napomena:** Za testiranje Librarian/Admin uloga, potrebno je ručno dodeliti ulogu preko baze:

```sql
-- Pronađite UserId korisnika
SELECT Id, Email FROM AspNetUsers WHERE Email = 'vas-email@example.com';

-- Pronađite RoleId za Librarian
SELECT Id FROM AspNetRoles WHERE Name = 'Librarian';

-- Dodajte ulogu
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('user-id-ovde', 'role-id-ovde');
```

---

## 📊 Baza podataka - Šema

### Glavne tabele:

#### Books
- Id, Title, Author, ISBN, Publisher, PublicationYear, Genre, Description
- TotalCopies, AvailableCopies

#### BookCopies
- Id, BookId (FK), CopyNumber, Status

#### Loans
- Id, BookCopyId (FK), UserId (FK)
- LoanDate, DueDate, ReturnDate, Status
- FineAmount, FinePaid

#### Reservations
- Id, BookId (FK), UserId (FK)
- ReservationDate, ExpirationDate, Status
- QueuePosition, NotifiedDate

#### AspNetUsers (Identity)
- Id, Email, UserName
- FirstName, LastName, DateOfBirth, Address
- IsBlocked, TotalFines

---

## 🔍 Ključne funkcionalnosti sistema

### 1. Automatski obračun kazni
- **Pravilo:** 50 RSD po danu kašnjenja
- **Implementacija:** `ReturnBookCommandHandler.cs`
- Automatski izračunava dane prekoračenja
- Dodaje kaznu na korisnički nalog
- Blokira pozajmljivanje ako ukupne kazne > 1000 RSD

### 2. Red čekanja za rezervacije (SLOŽENO)
- **FIFO princip:** Prvi prijavljeni, prvi dobija
- **Automatsko dodeljivanje:** Pri vraćanju knjige
- **Vremensko ograničenje:** 3 dana za preuzimanje
- **Dinamičko ažuriranje:** Otkazivanje pomera pozicije
- **Implementacija:** `CreateReservationCommandHandler.cs`, `ReturnBookCommandHandler.cs`

### 3. Blokada korisnika
- Automatska blokada pri dugovima > 1000 RSD
- Manuelna blokada od strane administratora
- Blokirani korisnici ne mogu pozajmljivati/rezervisati

### 4. Validacija ulaza
- FluentValidation na Application sloju
- Provera ISBN formata (10 ili 13 cifara)
- Provera datuma (godina izdanja)
- Provera broja primeraka (1-1000)

---

## 🔐 Sigurnost

### Autentifikacija:
- **MVC:** Cookie-based authentication (ASP.NET Core Identity)
- **API:** JWT Bearer tokens

### Autorizacija:
- Role-based (Reader, Librarian, Admin)
- Policy-based ("RequireLibrarianRole", "RequireAdminRole")

### Validacija:
- FluentValidation za biznis pravila
- Data Annotations za basic validaciju
- AntiForgeryToken za MVC forme

---

## 📁 Struktura projekta

```
LibraryManagementSystem/
├── LibraryManagementSystem.sln
├── README.md
└── src/
    ├── LibraryManagement.Domain/
    │   ├── Entities/
    │   ├── Enums/
    │   └── Interfaces/
    ├── LibraryManagement.Application/
    │   ├── Commands/
    │   ├── Queries/
    │   ├── Handlers/
    │   ├── DTOs/
    │   └── Validators/
    ├── LibraryManagement.Infrastructure/
    │   ├── Data/
    │   ├── Repositories/
    │   └── Identity/
    └── LibraryManagement.Web/
        ├── Controllers/
        │   ├── Api/
        │   └── Mvc/
        ├── Views/
        ├── Models/
        └── wwwroot/
```

---

## 🚀 API Endpoint-i (Swagger)

### Books API:
- `GET /api/BooksApi` - Lista svih knjiga (filter opcije)
- `GET /api/BooksApi/{id}` - Detalji knjige
- `POST /api/BooksApi` - Nova knjiga (Librarian+)
- `POST /api/BooksApi/borrow` - Pozajmi knjigu
- `POST /api/BooksApi/return` - Vrati knjigu
- `GET /api/BooksApi/loans/{userId}` - Pozajmice korisnika

### Reservations API:
- `POST /api/ReservationsApi` - Nova rezervacija
- `DELETE /api/ReservationsApi/{id}` - Otkaži rezervaciju
- `GET /api/ReservationsApi/queue/{bookId}` - Red čekanja
- `GET /api/ReservationsApi/user/{userId}` - Rezervacije korisnika

### Account API:
- `POST /api/AccountApi/register` - Registracija
- `POST /api/AccountApi/login` - Login (dobijate JWT token)

---

## ✅ Ispunjeni zahtevi projekta

- ✅ **ASP.NET aplikacija** (MVC + Web API)
- ✅ **5+ slučajeva korišćenja**
- ✅ **1 složen slučaj korišćenja** (Rezervacija sa redom čekanja)
- ✅ **Entity Framework Core** (SQL Server)
- ✅ **Autentifikacija i autorizacija** (Identity + JWT)
- ✅ **Slojevita arhitektura** (Domain, Application, Infrastructure, Web)
- ✅ **Dodatne biblioteke:**
  - MediatR (CQRS)
  - FluentValidation
  - AutoMapper
  - Swagger

---

## 📝 Napomene za asistenta

1. **Složen slučaj korišćenja:**
   - Implementiran u `CreateReservationCommandHandler.cs` (linija ~50-130)
   - Testira se: Rezervišite knjigu koja nema dostupnih primeraka, pa vratite neki primerak - prvi u redu automatski dobija notifikaciju

2. **CQRS pattern:**
   - Commands u `Application/Commands/`
   - Queries u `Application/Queries/`
   - Handlers u `Application/Handlers/`
   - Mediator koordinira sve

3. **Testiranje:**
   - Najlakše preko Swagger-a (`/swagger`)
   - Za MVC: Registrujte se pa testirajte kroz UI
   - Za admin/librarian: Ručno dodajte uloge u bazi (SQL gore u dokumentu)

4. **Baza podataka:**
   - Auto-kreiranje: `dotnet ef database update`
   - Ako baza postoji, možete je obrisati i ponovo kreirati
   - Seed data: Trenutno nema, ali se može dodati u `DbContext.OnModelCreating`

5. **API vs MVC:**
   - Oba rade istovremeno
   - API: `/api/...` (JWT autentifikacija)
   - MVC: `/Books`, `/Reservations` itd. (Cookie autentifikacija)

---

## 👨‍💻 Autor

**Radulović Mihailo**  
Broj indeksa: 2022/0542  
Email: mihailoradulovic99@gmail.com

---

## 📄 Licenca

Projekat kreiran u obrazovne svrhe za kurs Objektno-orijentisane tehnologije.

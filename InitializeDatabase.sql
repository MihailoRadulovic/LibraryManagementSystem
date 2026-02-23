-- =============================================
-- INICIJALIZACIJA BAZE PODATAKA
-- Sistem za upravljanje bibliotekom
-- =============================================

USE LibraryManagementDb;
GO

-- =============================================
-- 1. KREIRANJE ULOGA
-- =============================================
PRINT 'Kreiranje uloga...';

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
    PRINT '  - Admin uloga kreirana';
END

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Librarian')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Librarian', 'LIBRARIAN', NEWID());
    PRINT '  - Librarian uloga kreirana';
END

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Reader')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Reader', 'READER', NEWID());
    PRINT '  - Reader uloga kreirana';
END

PRINT 'Uloge uspešno kreirane!';
PRINT '';

-- =============================================
-- 2. SEED DATA - KNJIGE (OPCIONO)
-- =============================================
PRINT 'Dodavanje testnih knjiga...';

-- Napomena: Ovaj deo možete izvršiti ako želite testne podatke
-- Ili dodajte knjige preko aplikacije

PRINT 'Seed data završen!';
PRINT '';
PRINT '=============================================';
PRINT 'Inicijalizacija završena!';
PRINT 'Sada možete koristiti aplikaciju.';
PRINT '=============================================';

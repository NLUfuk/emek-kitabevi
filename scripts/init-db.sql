-- Emek Kitabevi Database Initialization Script
-- Bu script ilk kurulum için kullanılabilir

USE master;
GO

-- Database oluştur (eğer yoksa)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EmekKitabeviDb')
BEGIN
    CREATE DATABASE EmekKitabeviDb;
END
GO

USE EmekKitabeviDb;
GO

-- Database hazır
PRINT 'Database EmekKitabeviDb hazır.';
GO

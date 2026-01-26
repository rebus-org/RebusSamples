IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'showdowndb')
BEGIN
    CREATE DATABASE showdowndb;
END

-- ==========================================================
-- PROYECTO PRÁCTICO: SISTEMA DE AUTENTICACIÓN Y CONTROL DE ACCESO (LOGIN MVC)
-- Base de Datos: DB_LOGIN_PROYECT
-- Servidor: DESKTOP-ODBD2U2\SQLEXPRESS
-- ==========================================================

-- 1. Creación de la Base de Datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DB_LOGIN_PROYECT')
BEGIN
    CREATE DATABASE DB_LOGIN_PROYECT;
END
GO

USE DB_LOGIN_PROYECT;
GO

-- 2. Creación de la Tabla de Usuarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Nickname VARCHAR(50) NOT NULL UNIQUE,
        Correo VARCHAR(100) NOT NULL UNIQUE,
        Contrasena VARCHAR(255) NOT NULL
    );
END
GO

-- 3. Inserción de Registros Actuales / Pruebas / Integrantes
-- Usuario: admin
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Nickname = 'admin')
BEGIN
    INSERT INTO Usuarios (Nombre, Nickname, Correo, Contrasena)
    VALUES ('Administrador del Sistema', 'admin', 'admin@upana.edu.gt', 'admin123');
END
GO

-- Usuario: user (Roco)
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Nickname = 'user' OR Correo = 'roco@email.com')
BEGIN
    INSERT INTO Usuarios (Nombre, Nickname, Correo, Contrasena)
    VALUES ('User', 'user', 'roco@email.com', '12345');
END
GO

-- Usuario: rhenzo
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Nickname = 'rhenzo' OR Correo = 'rhenzo@email.com')
BEGIN
    INSERT INTO Usuarios (Nombre, Nickname, Correo, Contrasena)
    VALUES ('Rhenzo', 'rhenzo', 'rhenzo@email.com', '12345');
END
GO

-- 4. Verificación de los datos
SELECT IdUsuario, Nombre, Nickname, Correo, Contrasena FROM Usuarios;
GO

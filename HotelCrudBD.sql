USE [HotelCaliforniaDB]
GO
/****** Object:  Table [dbo].[Habitaciones]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Habitaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [nvarchar](10) NOT NULL,
	[Tipo] [nvarchar](30) NULL,
	[Precio] [decimal](10, 2) NULL,
	[Estado] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PersonaRol]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonaRol](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPersona] [int] NOT NULL,
	[IdRol] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personas]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Apellidos] [nvarchar](50) NOT NULL,
	[Correo] [nvarchar](100) NULL,
	[Telefono] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reservas]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPersona] [int] NOT NULL,
	[IdHabitacion] [int] NOT NULL,
	[FechaEntrada] [date] NULL,
	[FechaSalida] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Descripcion] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PersonaRol]  WITH CHECK ADD FOREIGN KEY([IdPersona])
REFERENCES [dbo].[Personas] ([Id])
GO
ALTER TABLE [dbo].[PersonaRol]  WITH CHECK ADD FOREIGN KEY([IdRol])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Reservas]  WITH CHECK ADD FOREIGN KEY([IdHabitacion])
REFERENCES [dbo].[Habitaciones] ([Id])
GO
ALTER TABLE [dbo].[Reservas]  WITH CHECK ADD FOREIGN KEY([IdPersona])
REFERENCES [dbo].[Personas] ([Id])
GO
/****** Object:  StoredProcedure [dbo].[sp_AddHabitacion]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AddHabitacion]
    @Numero NVARCHAR(10),
    @Tipo NVARCHAR(30),
    @Precio DECIMAL(10, 2),
    @Estado NVARCHAR(20)
AS
BEGIN
    INSERT INTO Habitaciones (Numero, Tipo, Precio, Estado)
    VALUES (@Numero, @Tipo, @Precio, @Estado);
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_AddPersona]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Agregar persona
CREATE PROCEDURE [dbo].[sp_AddPersona]
    @Nombre NVARCHAR(50),
    @Apellidos NVARCHAR(50),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(20)
AS
BEGIN
    INSERT INTO Personas (Nombre, Apellidos, Correo, Telefono)
    VALUES (@Nombre, @Apellidos, @Correo, @Telefono);
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_AddReserva]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AddReserva]
    @IdPersona INT,
    @IdHabitacion INT,
    @FechaEntrada DATE,
    @FechaSalida DATE
AS
BEGIN
    INSERT INTO Reservas (IdPersona, IdHabitacion, FechaEntrada, FechaSalida)
    VALUES (@IdPersona, @IdHabitacion, @FechaEntrada, @FechaSalida);

    -- Actualizar estado de la habitación a "Ocupada"
    UPDATE Habitaciones
    SET Estado = 'Ocupada'
    WHERE Id = @IdHabitacion;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_AddRol]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Agregar rol
CREATE PROCEDURE [dbo].[sp_AddRol]
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(200)
AS
BEGIN
    INSERT INTO Roles (Nombre, Descripcion)
    VALUES (@Nombre, @Descripcion);
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_AsignarRolAPersona]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AsignarRolAPersona]
    @IdPersona INT,
    @IdRol INT
AS
BEGIN
    INSERT INTO PersonaRol (IdPersona, IdRol)
    VALUES (@IdPersona, @IdRol);
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteHabitacion]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteHabitacion]
    @Id INT
AS
BEGIN
    DELETE FROM Habitaciones WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePersona]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Eliminar persona
CREATE PROCEDURE [dbo].[sp_DeletePersona]
    @Id INT
AS
BEGIN
    DELETE FROM Personas WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteReserva]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- eliminar reserva
CREATE PROCEDURE [dbo].[sp_DeleteReserva]
    @Id INT
AS
BEGIN
    DECLARE @IdHabitacion INT;

    -- Obtener la habitación relacionada
    SELECT @IdHabitacion = IdHabitacion FROM Reservas WHERE Id = @Id;

    -- Eliminar la reserva
    DELETE FROM Reservas WHERE Id = @Id;

    -- Cambiar el estado de la habitación a "Disponible"
    UPDATE Habitaciones
    SET Estado = 'Disponible'
    WHERE Id = @IdHabitacion;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteRol]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Eliminar rol
CREATE PROCEDURE [dbo].[sp_DeleteRol]
    @Id INT
AS
BEGIN
    DELETE FROM Roles WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_EliminarRolDePersona]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_EliminarRolDePersona]
    @IdPersona INT,
    @IdRol INT
AS
BEGIN
    DELETE FROM PersonaRol
    WHERE IdPersona = @IdPersona AND IdRol = @IdRol;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllHabitaciones]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllHabitaciones]
AS
BEGIN
    SELECT * FROM Habitaciones;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllPersonas]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Obtener todas las personas
CREATE PROCEDURE [dbo].[sp_GetAllPersonas]
AS
BEGIN
    SELECT * FROM Personas;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllReservas]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllReservas]
AS
BEGIN
    SELECT r.Id, p.Nombre + ' ' + p.Apellidos AS Cliente, h.Numero AS Habitacion,
           r.FechaEntrada, r.FechaSalida
    FROM Reservas r
    INNER JOIN Personas p ON r.IdPersona = p.Id
    INNER JOIN Habitaciones h ON r.IdHabitacion = h.Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllRoles]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Obtener todos los roles
CREATE PROCEDURE [dbo].[sp_GetAllRoles]
AS
BEGIN
    SELECT * FROM Roles;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetHabitacion]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetHabitacion]
    @Id INT
AS
BEGIN
    SELECT * FROM Habitaciones WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetReserva]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetReserva]
    @Id INT
AS
BEGIN
    SELECT * FROM Reservas WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetRolesPorPersona]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetRolesPorPersona]
    @IdPersona INT
AS
BEGIN
    SELECT r.*
    FROM Roles r
    INNER JOIN PersonaRol pr ON pr.IdRol = r.Id
    WHERE pr.IdPersona = @IdPersona;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateHabitacion]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateHabitacion]
    @Id INT,
    @Numero NVARCHAR(10),
    @Tipo NVARCHAR(30),
    @Precio DECIMAL(10, 2),
    @Estado NVARCHAR(20)
AS
BEGIN
    UPDATE Habitaciones
    SET Numero = @Numero,
        Tipo = @Tipo,
        Precio = @Precio,
        Estado = @Estado
    WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdatePersona]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Actualizar persona
CREATE PROCEDURE [dbo].[sp_UpdatePersona]
    @Id INT,
    @Nombre NVARCHAR(50),
    @Apellidos NVARCHAR(50),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(20)
AS
BEGIN
    UPDATE Personas
    SET Nombre = @Nombre,
        Apellidos = @Apellidos,
        Correo = @Correo,
        Telefono = @Telefono
    WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateReserva]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateReserva]
    @Id INT,
    @IdPersona INT,
    @IdHabitacion INT,
    @FechaEntrada DATE,
    @FechaSalida DATE
AS
BEGIN
    UPDATE Reservas
    SET IdPersona = @IdPersona,
        IdHabitacion = @IdHabitacion,
        FechaEntrada = @FechaEntrada,
        FechaSalida = @FechaSalida
    WHERE Id = @Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateRol]    Script Date: 5/7/2025 19:44:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Actualizar rol
CREATE PROCEDURE [dbo].[sp_UpdateRol]
    @Id INT,
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(200)
AS
BEGIN
    UPDATE Roles
    SET Nombre = @Nombre,
        Descripcion = @Descripcion
    WHERE Id = @Id;
END;
GO

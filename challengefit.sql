-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 21-04-2026 a las 19:37:17
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `challengefit`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `desafios`
--

CREATE TABLE `desafios` (
  `Id` int(11) NOT NULL,
  `Titulo` varchar(150) NOT NULL,
  `Descripcion` text NOT NULL,
  `FechaInicio` datetime NOT NULL,
  `FechaFin` datetime NOT NULL,
  `Puntos` int(11) NOT NULL DEFAULT 0,
  `IdEntrenador` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `desafios`
--

INSERT INTO `desafios` (`Id`, `Titulo`, `Descripcion`, `FechaInicio`, `FechaFin`, `Puntos`, `IdEntrenador`) VALUES
(1, '30 Días de Plancha', 'Aumenta tu resistencia y fortalece tu core completando el desafío de plancha diario durante un mes.', '2026-03-01 00:00:00', '2026-03-31 00:00:00', 500, 3),
(2, 'Correr 10km', 'Prepárate para correr 10 kilómetros en un mes. Sigue el plan de entrenamiento para lograrlo.', '2026-03-05 00:00:00', '2026-04-05 00:00:00', 300, 3),
(3, '500 Flexiones en una Semana', 'Un reto de fuerza y resistencia. Acumula 500 flexiones a lo largo de la semana. ¡Tú puedes!', '2026-03-14 00:00:00', '2026-03-21 00:00:00', 200, 3),
(4, 'Reto de Sentadillas', 'Completa 1000 sentadillas en 2 semanas. Fortalece tus piernas al máximo.', '2026-03-10 00:00:00', '2026-03-24 00:00:00', 400, 3),
(5, 'Semana de Cardio', 'Realiza 5 sesiones de cardio HIIT en una semana para mejorar tu resistencia cardiovascular.', '2026-03-17 00:00:00', '2026-03-24 00:00:00', 250, 3),
(6, 'prueba', 'datos prueba', '2026-04-03 00:00:00', '2026-04-04 00:00:00', 100, 3),
(7, 'desafio prueba', 'prueba', '2026-04-09 00:00:00', '2026-04-10 00:00:00', 400, 8);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `desafiousuarios`
--

CREATE TABLE `desafiousuarios` (
  `Id` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `IdDesafio` int(11) NOT NULL,
  `Progreso` int(11) NOT NULL DEFAULT 0,
  `Completado` tinyint(4) NOT NULL DEFAULT 0,
  `FechaAsignado` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `desafiousuarios`
--

INSERT INTO `desafiousuarios` (`Id`, `IdUsuario`, `IdDesafio`, `Progreso`, `Completado`, `FechaAsignado`) VALUES
(1, 4, 1, 83, 0, '2026-03-01 10:00:00'),
(2, 4, 2, 45, 0, '2026-03-05 10:00:00'),
(3, 4, 3, 70, 0, '2026-03-14 10:00:00'),
(4, 5, 1, 50, 0, '2026-03-01 10:00:00'),
(5, 5, 4, 100, 1, '2026-03-10 10:00:00'),
(6, 6, 2, 20, 0, '2026-03-05 10:00:00'),
(7, 6, 5, 60, 0, '2026-03-17 10:00:00'),
(8, 9, 7, 0, 0, '2026-04-09 01:10:39'),
(9, 10, 7, 0, 0, '2026-04-10 17:01:34');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `ejercicios`
--

CREATE TABLE `ejercicios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `GrupoMuscular` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `ejercicios`
--

INSERT INTO `ejercicios` (`Id`, `Nombre`, `GrupoMuscular`) VALUES
(1, 'Press de Banca', 'Pecho'),
(2, 'Sentadillas Libres', 'Piernas'),
(3, 'Peso Muerto Convencional', 'Espalda / Piernas'),
(4, 'Press Militar con Barra', 'Hombros'),
(5, 'Dominadas supinas', 'Espalda'),
(6, 'Remo con Barra', 'Espalda'),
(7, 'Curl de Bíceps con Barra', 'Brazos (Bíceps)'),
(8, 'Press Francés con Barra Z', 'Brazos (Tríceps)'),
(9, 'Zancadas con Mancuernas', 'Piernas'),
(10, 'Plancha Abdominal', 'Core / Abdominales'),
(11, 'Flexiones', 'Pecho'),
(12, 'Remo con mancuerna', 'Espalda'),
(13, 'Press de hombros', 'Hombros'),
(14, 'Sentadillas', 'Piernas'),
(15, 'Peso muerto', 'Piernas'),
(16, 'Curl de bíceps', 'Brazos'),
(17, 'Extensión de tríceps', 'Brazos'),
(18, 'Plancha', 'Core'),
(19, 'Abdominales', 'Core'),
(20, 'Burpees', 'Full body'),
(21, 'Zancadas', 'Piernas'),
(22, 'Dominadas', 'Espalda');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `especialidades`
--

CREATE TABLE `especialidades` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `especialidades`
--

INSERT INTO `especialidades` (`Id`, `Nombre`) VALUES
(1, 'Musculación'),
(2, 'Crossfit'),
(3, 'Yoga'),
(4, 'Pilates'),
(5, 'Cardio HIIT'),
(6, 'Nutrición deportiva'),
(7, 'Entrenamiento funcional'),
(8, 'Rehabilitación deportiva');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `especialidad_entrenador`
--

CREATE TABLE `especialidad_entrenador` (
  `Id` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `IdEspecialidad` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `especialidad_entrenador`
--

INSERT INTO `especialidad_entrenador` (`Id`, `IdUsuario`, `IdEspecialidad`) VALUES
(1, 3, 1),
(2, 3, 2),
(3, 3, 5),
(4, 3, 7),
(5, 8, 8);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `objetivos`
--

CREATE TABLE `objetivos` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `objetivos`
--

INSERT INTO `objetivos` (`Id`, `Nombre`) VALUES
(1, 'Ganar masa muscular'),
(2, 'Perder peso'),
(3, 'Mejorar resistencia'),
(4, 'Tonificar'),
(5, 'Mejorar flexibilidad'),
(6, 'Aumentar fuerza'),
(7, 'Rehabilitación'),
(8, 'Preparación deportiva');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `objetivo_alumno`
--

CREATE TABLE `objetivo_alumno` (
  `Id` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `IdObjetivo` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `objetivo_alumno`
--

INSERT INTO `objetivo_alumno` (`Id`, `IdUsuario`, `IdObjetivo`) VALUES
(1, 4, 1),
(2, 4, 6),
(3, 5, 2),
(4, 5, 4),
(5, 6, 3),
(6, 6, 8),
(7, 9, 7),
(8, 10, 6);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `progresos`
--

CREATE TABLE `progresos` (
  `Id` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `IdRutina` int(11) DEFAULT NULL,
  `FechaRegistro` datetime NOT NULL DEFAULT current_timestamp(),
  `Estadisticas` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL COMMENT 'Datos en formato JSON' CHECK (json_valid(`Estadisticas`)),
  `Completado` tinyint(4) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `progresos`
--

INSERT INTO `progresos` (`Id`, `IdUsuario`, `IdRutina`, `FechaRegistro`, `Estadisticas`, `Completado`) VALUES
(6, 4, NULL, '2026-03-13 08:05:00', '{\"duracion\": 48, \"nota\": \"Buena sesión\"}', 1),
(7, 4, NULL, '2026-03-14 08:05:00', '{\"duracion\": 42, \"nota\": \"Día de piernas intenso\"}', 1),
(8, 4, NULL, '2026-03-15 08:05:00', '{\"duracion\": 58, \"nota\": \"Full body completado\"}', 1),
(9, 4, NULL, '2026-03-16 08:05:00', '{\"duracion\": 30, \"nota\": \"Core y abdominales\"}', 1),
(10, 4, NULL, '2026-03-17 08:05:00', '{\"duracion\": 37, \"nota\": \"HIIT muy exigente\"}', 1),
(11, 5, NULL, '2026-03-14 08:10:00', '{\"duracion\": 45, \"nota\": \"Primera rutina completada\"}', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `rutinaejercicios`
--

CREATE TABLE `rutinaejercicios` (
  `Id` int(11) NOT NULL,
  `IdRutina` int(11) NOT NULL,
  `IdEjercicio` int(11) NOT NULL,
  `Series` int(11) NOT NULL DEFAULT 1,
  `Repeticiones` int(11) NOT NULL DEFAULT 1,
  `Completado` tinyint(4) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `rutinaejercicios`
--

INSERT INTO `rutinaejercicios` (`Id`, `IdRutina`, `IdEjercicio`, `Series`, `Repeticiones`, `Completado`) VALUES
(32, 9, 1, 3, 10, 1),
(33, 9, 4, 4, 10, 1),
(34, 9, 2, 3, 10, 1),
(36, 10, 2, 3, 10, 1),
(37, 10, 1, 3, 10, 1),
(38, 11, 2, 3, 10, 1),
(39, 11, 3, 3, 10, 0),
(41, 9, 4, 3, 12, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `rutinas`
--

CREATE TABLE `rutinas` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Nivel` varchar(50) NOT NULL,
  `Descripcion` text NOT NULL,
  `Duracion` int(11) NOT NULL COMMENT 'Duración en minutos',
  `IdEntrenador` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `rutinas`
--

INSERT INTO `rutinas` (`Id`, `Nombre`, `Nivel`, `Descripcion`, `Duracion`, `IdEntrenador`) VALUES
(9, 'rutinaprueba', 'Intermedio', 'de prueba', 60, 3),
(10, 'prueba arturo', 'Intermedio', 'prueba', 60, 8),
(11, 'rutina2', 'Intermedio', 'prueba', 60, 8);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `solicitudes`
--

CREATE TABLE `solicitudes` (
  `Id` int(11) NOT NULL,
  `IdAlumno` int(11) NOT NULL,
  `IdEntrenador` int(11) NOT NULL,
  `Estado` varchar(20) NOT NULL DEFAULT 'Pendiente' COMMENT '"Pendiente" | "Aceptada" | "Rechazada"',
  `FechaSolicitud` datetime NOT NULL DEFAULT current_timestamp(),
  `FechaRespuesta` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `solicitudes`
--

INSERT INTO `solicitudes` (`Id`, `IdAlumno`, `IdEntrenador`, `Estado`, `FechaSolicitud`, `FechaRespuesta`) VALUES
(1, 9, 8, 'Aceptada', '2026-04-07 23:43:08', '2026-04-08 00:08:07'),
(2, 10, 8, 'Rechazada', '2026-04-08 00:07:31', '2026-04-08 00:08:28'),
(3, 10, 8, 'Rechazada', '2026-04-08 19:14:57', '2026-04-08 19:43:57'),
(4, 10, 8, 'Aceptada', '2026-04-09 01:16:22', '2026-04-10 17:01:19');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuariorutinaejercicios`
--

CREATE TABLE `usuariorutinaejercicios` (
  `Id` int(11) NOT NULL,
  `IdUsuarioRutina` int(11) NOT NULL,
  `IdRutinaEjercicio` int(11) NOT NULL,
  `Completado` tinyint(4) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuariorutinaejercicios`
--

INSERT INTO `usuariorutinaejercicios` (`Id`, `IdUsuarioRutina`, `IdRutinaEjercicio`, `Completado`) VALUES
(1, 49, 38, 0),
(2, 49, 39, 0),
(3, 50, 32, 0),
(4, 50, 33, 0),
(5, 50, 34, 0),
(6, 50, 41, 0),
(7, 51, 32, 0),
(8, 51, 33, 0),
(9, 51, 34, 0),
(10, 51, 41, 0),
(11, 53, 38, 0),
(12, 53, 39, 0),
(16, 48, 36, 1),
(17, 48, 37, 1),
(18, 52, 32, 1),
(19, 52, 33, 1),
(20, 52, 34, 1),
(21, 52, 41, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuariorutinas`
--

CREATE TABLE `usuariorutinas` (
  `Id` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `IdRutina` int(11) NOT NULL,
  `FechaAsignacion` datetime NOT NULL DEFAULT current_timestamp(),
  `FechaFinalizacion` datetime DEFAULT NULL,
  `Completado` tinyint(4) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuariorutinas`
--

INSERT INTO `usuariorutinas` (`Id`, `IdUsuario`, `IdRutina`, `FechaAsignacion`, `FechaFinalizacion`, `Completado`) VALUES
(48, 9, 10, '2026-04-09 01:10:03', '2026-04-09 20:44:39', 1),
(49, 9, 11, '2026-04-09 21:03:33', NULL, 0),
(50, 6, 9, '2026-04-10 13:13:47', NULL, 0),
(51, 5, 9, '2026-04-10 13:13:50', NULL, 0),
(52, 4, 9, '2026-04-10 13:13:52', '2026-04-20 22:39:17', 1),
(53, 10, 11, '2026-04-10 17:01:31', NULL, 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `ClaveHash` varchar(255) NOT NULL,
  `Rol` varchar(20) NOT NULL COMMENT '"Entrenador" | "Alumno"',
  `Objetivo` varchar(255) DEFAULT NULL,
  `EntrenadorId` int(11) DEFAULT NULL COMMENT 'Solo para alumnos: FK al entrenador asignado'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Nombre`, `Email`, `ClaveHash`, `Rol`, `Objetivo`, `EntrenadorId`) VALUES
(3, 'Carlos Trainer', 'carlos@challengefit.com', 'AQAAAAIAAYagAAAAEAU88CEznl+nmnkqE+oIkA673NWaoM4HRXYls5a5CJJpJpBARQjqyQphCqBRu5HWHQ==', 'Entrenador', NULL, NULL),
(4, 'Juan Alumno', 'juan@gmail.com', 'AQAAAAIAAYagAAAAEAU88CEznl+nmnkqE+oIkA673NWaoM4HRXYls5a5CJJpJpBARQjqyQphCqBRu5HWHQ==', 'Alumno', 'Ganar masa muscular', 3),
(5, 'María Alumna', 'maria@gmail.com', 'AQAAAAIAAYagAAAAEAU88CEznl+nmnkqE+oIkA673NWaoM4HRXYls5a5CJJpJpBARQjqyQphCqBRu5HWHQ==', 'Alumno', 'Perder peso', 3),
(6, 'Pedro Alumno', 'pedro@gmail.com', 'AQAAAAIAAYagAAAAEAU88CEznl+nmnkqE+oIkA673NWaoM4HRXYls5a5CJJpJpBARQjqyQphCqBRu5HWHQ==', 'Alumno', 'Mejorar resistencia', 3),
(8, 'Arturo Prueba', 'arturo@gmail.com', 'AQAAAAIAAYagAAAAEE6d4WiWskvCilHwHeCXQiKyXFlcjPKHJ8k8AYQJUb8cl/1b3Ybhaaw+7zNRAMFwbQ==', 'Entrenador', NULL, NULL),
(9, 'Alumno', 'alumno@mail.com', 'AQAAAAIAAYagAAAAEAzqoGA85YP3Qe4P2s4U9cXHxEPE7boHM0Bfnxu+M7pK2hCXjIYclfE7c2Jf5URrFw==', 'Alumno', 'Rehabilitación', 8),
(10, 'alumno2', 'alumno2@mail.com', 'AQAAAAIAAYagAAAAEG4Q8R25cR0CsY9mc99RiO3XqQTllpmICPtBtKR080Xoir6BchM1EG5VaWAZzyYLOg==', 'Alumno', 'Aumentar fuerza', 8);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `desafios`
--
ALTER TABLE `desafios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_desafio_entrenador` (`IdEntrenador`);

--
-- Indices de la tabla `desafiousuarios`
--
ALTER TABLE `desafiousuarios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_desafiousuario_usuario` (`IdUsuario`),
  ADD KEY `fk_desafiousuario_desafio` (`IdDesafio`);

--
-- Indices de la tabla `ejercicios`
--
ALTER TABLE `ejercicios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `especialidades`
--
ALTER TABLE `especialidades`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `especialidad_entrenador`
--
ALTER TABLE `especialidad_entrenador`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_especialidadentrenador_usuario` (`IdUsuario`),
  ADD KEY `fk_especialidadentrenador_especialidad` (`IdEspecialidad`);

--
-- Indices de la tabla `objetivos`
--
ALTER TABLE `objetivos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `objetivo_alumno`
--
ALTER TABLE `objetivo_alumno`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_objetivoalumno_usuario` (`IdUsuario`),
  ADD KEY `fk_objetivoalumno_objetivo` (`IdObjetivo`);

--
-- Indices de la tabla `progresos`
--
ALTER TABLE `progresos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_progreso_usuario` (`IdUsuario`),
  ADD KEY `fk_progreso_rutina` (`IdRutina`);

--
-- Indices de la tabla `rutinaejercicios`
--
ALTER TABLE `rutinaejercicios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_rutinaejercicio_rutina` (`IdRutina`),
  ADD KEY `fk_rutinaejercicio_ejercicio` (`IdEjercicio`);

--
-- Indices de la tabla `rutinas`
--
ALTER TABLE `rutinas`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_rutina_entrenador` (`IdEntrenador`);

--
-- Indices de la tabla `solicitudes`
--
ALTER TABLE `solicitudes`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_solicitud_alumno` (`IdAlumno`),
  ADD KEY `fk_solicitud_entrenador` (`IdEntrenador`);

--
-- Indices de la tabla `usuariorutinaejercicios`
--
ALTER TABLE `usuariorutinaejercicios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_ure_usuariorutina` (`IdUsuarioRutina`),
  ADD KEY `fk_ure_rutinaejercicio` (`IdRutinaEjercicio`);

--
-- Indices de la tabla `usuariorutinas`
--
ALTER TABLE `usuariorutinas`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_usuariorutina_usuario` (`IdUsuario`),
  ADD KEY `fk_usuariorutina_rutina` (`IdRutina`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD KEY `fk_usuario_entrenador` (`EntrenadorId`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `desafios`
--
ALTER TABLE `desafios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `desafiousuarios`
--
ALTER TABLE `desafiousuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `ejercicios`
--
ALTER TABLE `ejercicios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT de la tabla `especialidades`
--
ALTER TABLE `especialidades`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `especialidad_entrenador`
--
ALTER TABLE `especialidad_entrenador`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `objetivos`
--
ALTER TABLE `objetivos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `objetivo_alumno`
--
ALTER TABLE `objetivo_alumno`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `progresos`
--
ALTER TABLE `progresos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `rutinaejercicios`
--
ALTER TABLE `rutinaejercicios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT de la tabla `rutinas`
--
ALTER TABLE `rutinas`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `solicitudes`
--
ALTER TABLE `solicitudes`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `usuariorutinaejercicios`
--
ALTER TABLE `usuariorutinaejercicios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT de la tabla `usuariorutinas`
--
ALTER TABLE `usuariorutinas`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=54;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `desafios`
--
ALTER TABLE `desafios`
  ADD CONSTRAINT `fk_desafio_entrenador` FOREIGN KEY (`IdEntrenador`) REFERENCES `usuarios` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `desafiousuarios`
--
ALTER TABLE `desafiousuarios`
  ADD CONSTRAINT `fk_desafiousuario_desafio` FOREIGN KEY (`IdDesafio`) REFERENCES `desafios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_desafiousuario_usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `especialidad_entrenador`
--
ALTER TABLE `especialidad_entrenador`
  ADD CONSTRAINT `fk_especialidadentrenador_especialidad` FOREIGN KEY (`IdEspecialidad`) REFERENCES `especialidades` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_especialidadentrenador_usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `objetivo_alumno`
--
ALTER TABLE `objetivo_alumno`
  ADD CONSTRAINT `fk_objetivoalumno_objetivo` FOREIGN KEY (`IdObjetivo`) REFERENCES `objetivos` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_objetivoalumno_usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `progresos`
--
ALTER TABLE `progresos`
  ADD CONSTRAINT `fk_progreso_rutina` FOREIGN KEY (`IdRutina`) REFERENCES `rutinas` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_progreso_usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `rutinaejercicios`
--
ALTER TABLE `rutinaejercicios`
  ADD CONSTRAINT `fk_rutinaejercicio_ejercicio` FOREIGN KEY (`IdEjercicio`) REFERENCES `ejercicios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rutinaejercicio_rutina` FOREIGN KEY (`IdRutina`) REFERENCES `rutinas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `rutinas`
--
ALTER TABLE `rutinas`
  ADD CONSTRAINT `fk_rutina_entrenador` FOREIGN KEY (`IdEntrenador`) REFERENCES `usuarios` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `solicitudes`
--
ALTER TABLE `solicitudes`
  ADD CONSTRAINT `fk_solicitud_alumno` FOREIGN KEY (`IdAlumno`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_solicitud_entrenador` FOREIGN KEY (`IdEntrenador`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `usuariorutinaejercicios`
--
ALTER TABLE `usuariorutinaejercicios`
  ADD CONSTRAINT `fk_ure_rutinaejercicio` FOREIGN KEY (`IdRutinaEjercicio`) REFERENCES `rutinaejercicios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_ure_usuariorutina` FOREIGN KEY (`IdUsuarioRutina`) REFERENCES `usuariorutinas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `usuariorutinas`
--
ALTER TABLE `usuariorutinas`
  ADD CONSTRAINT `fk_usuariorutina_rutina` FOREIGN KEY (`IdRutina`) REFERENCES `rutinas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_usuariorutina_usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD CONSTRAINT `fk_usuario_entrenador` FOREIGN KEY (`EntrenadorId`) REFERENCES `usuarios` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

-- =====================================================
-- Script para crear la tabla usuariorutinaejercicios
-- y poblar datos iniciales para asignaciones existentes
-- Ejecutar en phpMyAdmin sobre la base "challengefit"
-- =====================================================

-- 1. Crear la tabla
CREATE TABLE `usuariorutinaejercicios` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `IdUsuarioRutina` int(11) NOT NULL,
  `IdRutinaEjercicio` int(11) NOT NULL,
  `Completado` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  KEY `fk_ure_usuariorutina` (`IdUsuarioRutina`),
  KEY `fk_ure_rutinaejercicio` (`IdRutinaEjercicio`),
  CONSTRAINT `fk_ure_usuariorutina` FOREIGN KEY (`IdUsuarioRutina`) REFERENCES `usuariorutinas` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_ure_rutinaejercicio` FOREIGN KEY (`IdRutinaEjercicio`) REFERENCES `rutinaejercicios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- 2. Poblar datos para asignaciones NO completadas (progreso en 0)
INSERT INTO `usuariorutinaejercicios` (`IdUsuarioRutina`, `IdRutinaEjercicio`, `Completado`)
SELECT ur.Id, re.Id, 0
FROM usuariorutinas ur
INNER JOIN rutinaejercicios re ON re.IdRutina = ur.IdRutina
WHERE ur.Completado = 0;

-- 3. Poblar datos para asignaciones YA completadas (progreso en 1)
INSERT INTO `usuariorutinaejercicios` (`IdUsuarioRutina`, `IdRutinaEjercicio`, `Completado`)
SELECT ur.Id, re.Id, 1
FROM usuariorutinas ur
INNER JOIN rutinaejercicios re ON re.IdRutina = ur.IdRutina
WHERE ur.Completado = 1;

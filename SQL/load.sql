-- =========================
-- SCRIPT DE PRECARGA ALPU 2026
-- =========================
-- Este script contiene datos de ejemplo basados en los aranceles 2026 de ALPU

-- =========================
-- COUNTRIES & REGIONS
-- =========================

INSERT INTO region ( multiplier) VALUES
(1.5),
(1.85),
(2.25),
(2.5);

INSERT INTO country (country_code, name, region_id) VALUES
('UY', 'Uruguay', 1),
('AR', 'Argentina', 1),
('CL', 'Chile', 1),
('PY', 'Paraguay', 1),
('BR', 'Brasil', 2),
('MX', 'Mexico', 3),
('ES', 'España', 4);

-- =========================
-- DEPARTMENTS
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(1,  'Artigas', 'UY'),
(2,  'Canelones', 'UY'),
(3,  'Cerro Largo', 'UY'),
(4,  'Colonia', 'UY'),
(5,  'Durazno', 'UY'),
(6,  'Flores', 'UY'),
(7,  'Florida', 'UY'),
(8,  'Lavalleja', 'UY'),
(9,  'Maldonado', 'UY'),
(10, 'Montevideo', 'UY'),
(11, 'Paysandú', 'UY'),
(12, 'Río Negro', 'UY'),
(13, 'Rivera', 'UY'),
(14, 'Rocha', 'UY'),
(15, 'Salto', 'UY'),
(16, 'San José', 'UY'),
(17, 'Soriano', 'UY'),
(18, 'Tacuarembó', 'UY'),
(19, 'Treinta y Tres', 'UY');

-- =========================
-- ARGENTINA (Provincias)
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(20, 'Buenos Aires', 'AR'),
(21, 'Ciudad Autónoma de Buenos Aires', 'AR'),
(22, 'Catamarca', 'AR'),
(23, 'Chaco', 'AR'),
(24, 'Chubut', 'AR'),
(25, 'Córdoba', 'AR'),
(26, 'Corrientes', 'AR'),
(27, 'Entre Ríos', 'AR'),
(28, 'Formosa', 'AR'),
(29, 'Jujuy', 'AR'),
(30, 'La Pampa', 'AR'),
(31, 'La Rioja', 'AR'),
(32, 'Mendoza', 'AR'),
(33, 'Misiones', 'AR'),
(34, 'Neuquén', 'AR'),
(35, 'Río Negro', 'AR'),
(36, 'Salta', 'AR'),
(37, 'San Juan', 'AR'),
(38, 'San Luis', 'AR'),
(39, 'Santa Cruz', 'AR'),
(40, 'Santa Fe', 'AR'),
(41, 'Santiago del Estero', 'AR'),
(42, 'Tierra del Fuego, Antártida e Islas del Atlántico Sur', 'AR'),
(43, 'Tucumán', 'AR');

-- =========================
-- CHILE (Regiones)
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(44, 'Arica y Parinacota', 'CL'),
(45, 'Tarapacá', 'CL'),
(46, 'Antofagasta', 'CL'),
(47, 'Atacama', 'CL'),
(48, 'Coquimbo', 'CL'),
(49, 'Valparaíso', 'CL'),
(50, 'Región del Libertador Gral. Bernardo O’Higgins', 'CL'),
(51, 'Región del Maule', 'CL'),
(52, 'Región de Ñuble', 'CL'),
(53, 'Región del Biobío', 'CL'),
(54, 'Región de la Araucanía', 'CL'),
(55, 'Región de Los Ríos', 'CL'),
(56, 'Región de Los Lagos', 'CL'),
(57, 'Región de Aysén del Gral. Carlos Ibáñez del Campo', 'CL'),
(58, 'Región de Magallanes y de la Antártica Chilena', 'CL'),
(59, 'Región Metropolitana de Santiago', 'CL');

-- =========================
-- PARAGUAY (Departamentos)
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(60, 'Asunción (Distrito Capital)', 'PY'),
(61, 'Concepción', 'PY'),
(62, 'San Pedro', 'PY'),
(63, 'Cordillera', 'PY'),
(64, 'Guairá', 'PY'),
(65, 'Caaguazú', 'PY'),
(66, 'Caazapá', 'PY'),
(67, 'Itapúa', 'PY'),
(68, 'Misiones', 'PY'),
(69, 'Paraguarí', 'PY'),
(70, 'Alto Paraná', 'PY'),
(71, 'Central', 'PY'),
(72, 'Ñeembucú', 'PY'),
(73, 'Amambay', 'PY'),
(74, 'Canindeyú', 'PY'),
(75, 'Presidente Hayes', 'PY'),
(76, 'Alto Paraguay', 'PY'),
(77, 'Boquerón', 'PY');

-- =========================
-- BRASIL (Estados)
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(78, 'Acre', 'BR'),
(79, 'Alagoas', 'BR'),
(80, 'Amapá', 'BR'),
(81, 'Amazonas', 'BR'),
(82, 'Bahia', 'BR'),
(83, 'Ceará', 'BR'),
(84, 'Distrito Federal', 'BR'),
(85, 'Espírito Santo', 'BR'),
(86, 'Goiás', 'BR'),
(87, 'Maranhão', 'BR'),
(88, 'Mato Grosso', 'BR'),
(89, 'Mato Grosso do Sul', 'BR'),
(90, 'Minas Gerais', 'BR'),
(91, 'Pará', 'BR'),
(92, 'Paraíba', 'BR'),
(93, 'Paraná', 'BR'),
(94, 'Pernambuco', 'BR'),
(95, 'Piauí', 'BR'),
(96, 'Rio de Janeiro', 'BR'),
(97, 'Rio Grande do Norte', 'BR'),
(98, 'Rio Grande do Sul', 'BR'),
(99, 'Rondônia', 'BR'),
(100, 'Roraima', 'BR'),
(101, 'Santa Catarina', 'BR'),
(102, 'São Paulo', 'BR'),
(103, 'Sergipe', 'BR'),
(104, 'Tocantins', 'BR');

-- =========================
-- MÉXICO (Estados)
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(105, 'Aguascalientes', 'MX'),
(106, 'Baja California', 'MX'),
(107, 'Baja California Sur', 'MX'),
(108, 'Campeche', 'MX'),
(109, 'Chiapas', 'MX'),
(110, 'Chihuahua', 'MX'),
(111, 'Ciudad de México', 'MX'),
(112, 'Coahuila', 'MX'),
(113, 'Colima', 'MX'),
(114, 'Durango', 'MX'),
(115, 'Estado de México', 'MX'),
(116, 'Guanajuato', 'MX'),
(117, 'Guerrero', 'MX'),
(118, 'Hidalgo', 'MX'),
(119, 'Jalisco', 'MX'),
(120, 'Michoacán', 'MX'),
(121, 'Morelos', 'MX'),
(122, 'Nayarit', 'MX'),
(123, 'Nuevo León', 'MX'),
(124, 'Oaxaca', 'MX'),
(125, 'Puebla', 'MX'),
(126, 'Querétaro', 'MX'),
(127, 'Quintana Roo', 'MX'),
(128, 'San Luis Potosí', 'MX'),
(129, 'Sinaloa', 'MX'),
(130, 'Sonora', 'MX'),
(131, 'Tabasco', 'MX'),
(132, 'Tamaulipas', 'MX'),
(133, 'Tlaxcala', 'MX'),
(134, 'Veracruz', 'MX'),
(135, 'Yucatán', 'MX'),
(136, 'Zacatecas', 'MX');

-- =========================
-- ESPAÑA (Comunidades Autónomas)
-- =========================

INSERT INTO department (department_id, name, country_code) VALUES
(137, 'Andalucía', 'ES'),
(138, 'Aragón', 'ES'),
(139, 'Principado de Asturias', 'ES'),
(140, 'Illes Balears', 'ES'),
(141, 'Canarias', 'ES'),
(142, 'Cantabria', 'ES'),
(143, 'Castilla y León', 'ES'),
(144, 'Castilla-La Mancha', 'ES'),
(145, 'Cataluña', 'ES'),
(146, 'Comunitat Valenciana', 'ES'),
(147, 'Extremadura', 'ES'),
(148, 'Galicia', 'ES'),
(149, 'Comunidad de Madrid', 'ES'),
(150, 'Región de Murcia', 'ES'),
(151, 'Comunidad Foral de Navarra', 'ES'),
(152, 'País Vasco', 'ES'),
(153, 'La Rioja', 'ES'),
(154, 'Ceuta', 'ES'),
(155, 'Melilla', 'ES');

-- =========================
-- ADDRESSES
-- =========================

INSERT INTO address (country_code, department_id, city, street) VALUES
('UY', 1, 'Montevideo', 'Avenida 18 de Julio 1000'),
('UY', 3, 'Colonia', 'Calle Sarandí 500'),
('AR', 4, 'Buenos Aires', 'Avenida Corrientes 2000'),
('CL', 5, 'Santiago', 'Paseo Ahumada 100'),
('BR', 8, 'São Paulo', 'Avenida Paulista 1578'),
('MX', 11, 'Ciudad de México', 'Avenida Reforma 222'),
('ES', 15, 'Madrid', 'Calle Gran Vía 32'),
('UY', 10, 'Montevideo', 'Avenida Italia 2345'),
('UY', 10, 'Montevideo', 'Calle Colonia 1234'),
('UY', 10, 'Montevideo', 'Bulevar Artigas 890'),
('UY', 10, 'Montevideo', 'Calle Rivera 456'),
('UY', 10, 'Montevideo', 'Avenida Brasil 1100'),
('UY', 10, 'Montevideo', 'Calle Ejido 678');

-- =========================
-- USERS (ADMINS)
-- =========================

INSERT INTO "user" (google_id, first_name, last_name, email, password, address_id, state, rut, gender, identity_card) VALUES
(NULL, 'Admin', 'Principal', 'admin1@alpu.com', '$2a$11$HMTzHBnwyjc1H7X2.lj0Uus7kDGE/3DRJb3npDeaYHxGSdjzSop5i', 8, 'enabled', '102938475601', 'non_specified', '10000010'),
(NULL, 'Admin', 'Secundario', 'admin2@alpu.com', '$2a$11$HMTzHBnwyjc1H7X2.lj0Uus7kDGE/3DRJb3npDeaYHxGSdjzSop5i', 9, 'enabled', '102938475602', 'non_specified', '10000011'),
(NULL, 'Supervisor', 'Principal', 'supervisor1@alpu.com', '$2a$11$SSbzzeJQo16MF2g5LrB8LuJCf/Rd5DOge4bMckljIJM/Nv42ZWCme', 10, 'enabled', '102938475603', 'non_specified', '10000012'),
(NULL, 'Supervisor', 'Secundario', 'supervisor2@alpu.com', '$2a$11$SSbzzeJQo16MF2g5LrB8LuJCf/Rd5DOge4bMckljIJM/Nv42ZWCme', 11, 'enabled', '102938475604', 'non_specified', '10000013'),
(NULL, 'Contador', 'Principal', 'contador1@alpu.com', '$2a$11$dq4Tl8DObJpJEFhwRc2woeQ4R/eKYoxAKM1F6QqnRcVoTqfQ36T6C', 12, 'enabled', '102938475605', 'non_specified', '10000014'),
(NULL, 'Contador', 'Secundario', 'contador2@alpu.com', '$2a$11$dq4Tl8DObJpJEFhwRc2woeQ4R/eKYoxAKM1F6QqnRcVoTqfQ36T6C', 13, 'enabled', '102938475606', 'non_specified', '10000015');

INSERT INTO administrator (user_id) VALUES (1), (2);

INSERT INTO supervisor (user_id) VALUES (3), (4);

INSERT INTO accountant (user_id) VALUES (5), (6);

-- =========================
-- USERS (BROADCASTERS)
-- =========================

INSERT INTO "user" (google_id, first_name, last_name, email, password, address_id, state, rut, gender, identity_card) VALUES
(NULL, 'Juan', 'García', 'juan.garcia@alpu.com', '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 1, 'enabled', '102938475612', 'male', '10000016'),
(NULL, 'María', 'López', 'maria.lopez@alpu.com', '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 2, 'enabled', '293847561023', 'female', '10000017'),
(NULL, 'Carlos', 'Martínez', 'carlos.martinez@alpu.com', '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 3, 'enabled', '38475610293B', 'male', NULL),
(NULL, 'Ana', 'González', 'ana.gonzalez@alpu.com', '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 4, 'enabled', '475610293844', 'female', NULL),
(NULL, 'Roberto', 'Fernández', 'roberto.fernandez@alpu.com', '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 5, 'enabled', '56102938475K', 'male', NULL);

-- =========================
-- BROADCASTER CATEGORIES
-- =========================

INSERT INTO broadcaster_category (name, lifetime_job_count) VALUES
('Principiante', 0),
('Profesional', 4),
('Master', 10);

-- =========================
-- BROADCASTERS
-- =========================

INSERT INTO broadcaster (user_id, category_id) VALUES
(7, 2),
(8, 2),
(9, 1),
(10, 3),
(11, 2);


-- =========================
-- SKILLS
-- =========================

INSERT INTO skill (name) VALUES
('Actuación'),
('Maestro de ceremonias'),
('Locución a cámara'),
('Conducción radio'),
('Conducción TV'),
('Doblajes'),
('Español neutro'),
('Inglés británico');

-- =========================
-- LANGUAGES
-- =========================

INSERT INTO "language" (name) VALUES
('Español'),
('Inglés'),
('Francés'),
('Alemán'),
('Italiano');

-- Juan García (7): 0 skills
-- María López (8): 1 skill
-- Carlos Martínez (9): 2 skills
-- Ana González (10): 3 skills
-- Roberto Fernández (11): 0 skills
INSERT INTO broadcaster_skills (broadcaster_id, skill_id) VALUES
(8, 2),
(9, 5),
(9, 6),
(10, 1),
(10, 3),
(10, 7);

-- Juan García (7): 2 idiomas
-- María López (8): 0 idiomas
-- Carlos Martínez (9): 1 idioma
-- Ana González (10): 3 idiomas
-- Roberto Fernández (11): 0 idiomas
INSERT INTO broadcaster_languages (broadcaster_id, language_id) VALUES
(7, 1),
(7, 2),
(9, 1),
(10, 1),
(10, 3),
(10, 4);
-- =========================
-- MEMBERSHIP
-- =========================

INSERT INTO membership (broadcaster_id, pay_date, due_date, state, amount) VALUES
(7, '2026-04-01', '2027-05-01', 'valid', 500.00),
(8, '2026-04-05', '2027-05-05', 'valid', 500.00),
(9, '2026-03-01', '2027-04-01', 'expired', 500.00),
(10, '2026-04-10', '2027-06-10', 'valid', 500.00),
(11, '2026-02-01', '2027-03-01', 'expired', 500.00);

-- =========================
-- USERS (CLIENTS)
-- =========================

INSERT INTO "user" (first_name, last_name, email, password, address_id, state, rut, gender, identity_card) VALUES
('Empresa', 'Publicidad A', 'agencia1@example.com', '$2a$11$6M9rPx1gI4pBSVZygaXtfurHWD9oMnRlZEVHKVEGexKf6Gjm1xHca', 6, 'enabled', '12345678901A', NULL, NULL),
('Studio', 'Producciones', 'studio@example.com', '$2a$11$6M9rPx1gI4pBSVZygaXtfurHWD9oMnRlZEVHKVEGexKf6Gjm1xHca', 7, 'enabled', '987654321098', NULL, NULL);

-- =========================
-- AGENCY
-- =========================

INSERT INTO agency (name) VALUES
('Agencia de Publicidad Premier'),
('Studio de Producciones Creativas');

-- =========================
-- CLIENT
-- =========================

INSERT INTO client (user_id, agency_id) VALUES
(12, 1),
(13, 2);

-- =========================
-- SERVICE
-- =========================

INSERT INTO service (name, discriminator, type, base_price, extra_price, role_price) VALUES
('NARRACIONES, DOCUMENTALES, AUDIOVISUALES', 'narrative', 'narrative', 6800, 700, 2050);

-- =========================
-- IVR
-- =========================

INSERT INTO service (name, discriminator, type, base_price, extra_price, update_message_price) VALUES
('CONTESTADORES TELEFONICOS - IVR', 'ivr', 'ivr', 8200, 4100, 3100);

INSERT INTO range_ivr (service_id, min_word, max_word, price_per_word) VALUES
(2, 1, 100, 21),
(2, 101, 200, 19),
(2, 201, NULL, 17);

INSERT INTO service (name, discriminator, type, base_price) VALUES
('TELEVISIÓN', 'period', 'tv_generic', NULL),
('ZÓCALO TELEVISIÓN', 'period', 'tv_zocalo', NULL),
('PRESENTACIÓN DE PROGRAMAS - TELEVISIÓN', 'period', 'tv_host', NULL),
('RADIO', 'period', 'radio_generic', NULL),
('ZÓCALO RADIO', 'period', 'radio_zocalo', NULL),
('PRESENTACIÓN DE PROGRAMAS - RADIO', 'period', 'radio_host', NULL),
('INTERNET - PIEZAS DE VIDEO', 'period', 'internet_video', NULL),
('INTERNET - PIEZAS DE AUDIO', 'period', 'internet_audio', NULL),
('OTROS MEDIOS - VIDEO', 'period', 'others_video', NULL),
('OTROS MEDIOS - AUDIO', 'period', 'others_audio', NULL),
('CINE', 'period', 'cinema', NULL),
('NOTICIA EMPRESARIAL', 'period', 'others', 8200),
('MAESTRO DE CEREMONIAS - APERTURA', 'date', 'event', 16300),
('MAESTRO DE CEREMONIAS - COMPLETO', 'date', 'event', 23800),
('CONDUCCIÓN DE SHOW', 'date', 'event', 26100);

INSERT INTO service_period (service_id, interval, base_price, extra_price) VALUES
(3, 'one_week', 14700, 5900),
(3, 'one_month', 21200, 8500),
(3, 'three_months', 24500, 9800),
(3, 'six_months', 29300, 11700),
(3, 'one_year', 32600, 13000),
(4, 'one_week', 5900, NULL),
(4, 'one_month', 8500, NULL),
(4, 'three_months', 9800, NULL),
(4, 'six_months', 11700, NULL),
(4, 'one_year', 13000, NULL),
(5, 'one_week', 7300, NULL),
(5, 'one_month', 10600, NULL),
(5, 'three_months', 12200, NULL),
(5, 'six_months', 14700, NULL),
(5, 'one_year', 16300, NULL),
(6,'one_week', 6600, 2600),
(6,'one_month', 9600, 3800),
(6,'three_months', 11000, 4400),
(6,'six_months', 13200, 5300),
(6,'one_year', 14700, 5900),
(7, 'one_week', 2700, NULL),
(7, 'one_month', 3800, NULL),
(7, 'three_months', 4400, NULL),
(7, 'six_months', 5300, NULL),
(7, 'one_year', 5900, NULL),
(8, 'one_week', 3300, NULL),
(8, 'one_month', 4800, NULL),
(8, 'three_months', 5600, NULL),
(8, 'six_months', 6700, NULL),
(8, 'one_year', 7400, NULL),
(9, 'one_week', 10300, 4100),
(9, 'one_month', 14800, 5900),
(9, 'three_months', 17100, 6800),
(9, 'six_months', 20500, 8200),
(9, 'one_year', 22800, 9100),
(10, 'one_week', 4600, 1800),
(10, 'one_month', 6700, 2700),
(10, 'three_months', 7700, 3100),
(10, 'six_months', 9300, 3700),
(10, 'one_year', 10300, 4100),
(11, 'one_week', 5100, 2100),
(11, 'one_month', 7400, 3000),
(11, 'three_months', 8600, 3500),
(11, 'six_months', 10300, 4100),
(11, 'one_year', 11400, 4600),
(12, 'one_week', 2300, 900),
(12, 'one_month', 3300, 1300),
(12, 'three_months', 3800, 1500),
(12, 'six_months', 4600, 1800),
(12, 'one_year', 5100, 2000),
(13, 'one_week', 7300, 2900),
(13, 'one_month', 10600, 4200),
(13, 'three_months', 12200, 4900),
(13, 'six_months', 14700, 5900),
(13, 'one_year', 16300, 6500),
(14, 'one_week', 8200, NULL);

INSERT INTO service (name, discriminator, type) VALUES
('LOCUCIÓN A CÁMARA - PROTAGÓNICO', 'period', 'camera'),
('LOCUCIÓN A CÁMARA - CO-PROTAGÓNICO', 'period', 'camera'),
('LOCUCIÓN A CÁMARA - SECUNDARIO', 'period', 'camera'),
('LOCUCIÓN A CÁMARA - EXTRA CALIFICADO', 'period', 'camera');

INSERT INTO service_period (service_id, interval, base_price, extra_price, first_extra_price) VALUES
(18, 'one_week', 17600, 8800, 13200),
(18, 'one_month', 25400, 12700, 19000),
(18, 'three_months', 29300, 14700, 22000),
(18, 'six_months', 35200, 17600, 26400),
(18, 'one_year', 39100, 19600, 29300),
(19, 'one_week', 14100, 8800, 13200),
(19, 'one_month', 20300, 12700, 19000),
(19, 'three_months', 23500, 14700, 22000),
(19, 'six_months', 28200, 17600, 26400),
(19, 'one_year', 31300, 19600, 29300),
(20, 'one_week', 10600, 8800, 13200),
(20, 'one_month', 15300, 12700, 19000),
(20, 'three_months', 17600, 14700, 22000),
(20, 'six_months', 21200, 17600, 26400),
(20, 'one_year', 23500, 19600, 29300),
(21, 'one_week', 3500, 8800, 13200),
(21, 'one_month', 5100, 12700, 19000),
(21, 'three_months', 5900, 14700, 22000),
(21, 'six_months', 7000, 17600, 26400),
(21, 'one_year', 7800, 19600, 29300);

-- =========================
-- VOLUME DISCOUNTS
-- =========================

-- Radio
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'radio_generic', 4, NULL, 'percentage', 0.95),   -- 5% descuento 4 piezas o más
('Descuento por piezas', 'radio_generic', 6, NULL, 'percentage', 0.90),   -- 10% descuento 6 piezas o más
('Descuento por piezas', 'radio_generic', 9, NULL, 'percentage', 0.85);   -- 15% descuento 9 piezas o más

-- Televisión
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'tv_generic', 4, NULL, 'percentage', 0.90),   -- 10% descuento 4 piezas o más
('Descuento por piezas', 'tv_generic', 6, NULL, 'percentage', 0.85),   -- 15% descuento 6 piezas o más
('Descuento por piezas', 'tv_generic', 9, NULL, 'percentage', 0.80);   -- 20% descuento 9 piezas o más

-- Internet Audio
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'internet_audio', 4, NULL, 'percentage', 0.95),   -- 5% descuento 4 piezas o más
('Descuento por piezas', 'internet_audio', 6, NULL, 'percentage', 0.90),   -- 10% descuento 6 piezas o más
('Descuento por piezas', 'internet_audio', 9, NULL, 'percentage', 0.85);   -- 15% descuento 9 piezas o más

-- Internet Video
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'internet_video', 4, NULL, 'percentage', 0.90),   -- 10% descuento 4 piezas o más
('Descuento por piezas', 'internet_video', 6, NULL, 'percentage', 0.85),   -- 15% descuento 6 piezas o más
('Descuento por piezas', 'internet_video', 9, NULL, 'percentage', 0.80);   -- 20% descuento 9 piezas o más

-- Cine
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'cinema', 4, NULL, 'percentage', 0.90),   -- 10% descuento 4 piezas o más
('Descuento por piezas', 'cinema', 6, NULL, 'percentage', 0.85),   -- 15% descuento 6 piezas o más
('Descuento por piezas', 'cinema', 9, NULL, 'percentage', 0.80);   -- 20% descuento 9 piezas o más

-- Otros Medios Audio
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'others_audio', 4, NULL, 'percentage', 0.95),   -- 5% descuento 4 piezas o más
('Descuento por piezas', 'others_audio', 6, NULL, 'percentage', 0.90),   -- 10% descuento 6 piezas o más
('Descuento por piezas', 'others_audio', 9, NULL, 'percentage', 0.85);   -- 15% descuento 9 piezas o más

-- Otros Medios Video
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por piezas', 'others_video', 4, NULL, 'percentage', 0.90),   -- 10% descuento 4 piezas o más
('Descuento por piezas', 'others_video', 6, NULL, 'percentage', 0.85),   -- 15% descuento 6 piezas o más
('Descuento por piezas', 'others_video', 9, NULL, 'percentage', 0.80);   -- 20% descuento 9 piezas o más

-- Narraciones
INSERT INTO volume_discount (name, service_type, min_quantity, max_quantity, type, amount) VALUES
('Descuento por tiempo', 'narrative', 45, NULL, 'percentage', 0.90),   -- 10% descuento 45 minutos o más
('Descuento por tiempo', 'narrative', 60, NULL, 'percentage', 0.85),   -- 15% descuento 60 minutos o más
('Descuento por tiempo', 'narrative', 90, NULL, 'percentage', 0.80),   -- 20% descuento 90 minutos o más
('Descuento por tiempo', 'narrative', 150, NULL, 'percentage', 0.75);  -- 25% descuento 150 minutos o más 

-- =========================
-- MULTI SERVICE DISCOUNTS
-- =========================

INSERT INTO multi_service_discount (name, key, service_a, service_b, type, amount, is_discount_for_service_b_only) VALUES
('Pack TV+Radio', 'pack_tv_radio', 'tv_generic', 'radio_generic', 'percentage', 0.85, TRUE),
('Pack TV+Cine', 'pack_tv_cinema', 'tv_generic', 'cinema', 'percentage', 0.5, TRUE),
('Pack TV+Internet', 'pack_tv_internet', 'tv_generic', 'internet_video', 'percentage', 0.5, TRUE),
('Pack TV+Internet', 'pack_tv_internet', 'tv_generic', 'internet_audio', 'percentage', 0.5, TRUE);

-- =========================
-- PRICE ADJUSTMENTS
-- =========================

INSERT INTO price_adjustment (name, type, amount, key) VALUES
('Recargo Difusión en Medios Masivos', 'percentage', 1.30, 'for_mass_broadcast'), 		-- 30% de recargo para difusión en medios masivos (Maestro de ceremonias)
('Recargo Sincro Labial', 'percentage', 1.20, 'has_lip_sync'), 							-- 20% de recargo para trabajos que requieran sincro labial (Narraciones)
('Recargo Difusión en Internet', 'percentage', 2.00, 'on_internet'),		 			-- 100% de recargo para difusión en Internet (Narraciones)
-- ('Descuento TV + RADIO', 'percentage', 0.15, 'pack_tv_radio'),  						-- 15% de descuento en el iteam RADIO
-- ('Descuento TV + CINE', 'percentage', 0.50, 'pack_tv_cinema'), 						-- 50% de descuento en el item CINE
-- ('Descuento TV + INTERNET', 'percentage', 0.50, 'pack_tv_internet'), 				-- 50% de descuento en el item INTERNET
('Descuento Contado (10 días)', 'percentage', 0.90, 'in_cash'), 						-- 10% de descuento para pagos al contado
('Descuento Locutor Novel', 'percentage', 0.50, 'new_broadcaster'), 					-- 50% de descuento para locutores de categoría NOVEL en sus primeros 4 trabajos
('Descuento Participación Breve', 'percentage', 0.40, 'is_brief'), 						-- 60% de descuento para participaciones breves (menos de 5 palabras)
('Descuento Interior', 'percentage', 0.30, 'is_interior'),		 						-- 70% de descuento para trabajos realizados para el interior del país (solo para items Radio, Television y Cine)
('Descuento Participación Numerosa', 'percentage', 0.90, 'has_multiple_broadcasters'), 	-- 10% de descuento para participaciones breves cuando tres o mas locutres participan en una misma pieza
('Descuento Contenido No Comercial', 'percentage', 0.80, 'is_non_commercial'),			-- 20% de descuento para contenido no comercial (aplicable solo para narraciones)
('Descuento Uso Interno', 'percentage', 0.50, 'for_internal_use'), 						-- 50% de descuento para contenido de uso interno (aplicable solo para locucion a camara)
('Recargo por actualización', 'fixed', 3100, 'updates'),							-- 50% de descuento para contenido de uso interno (aplicable solo para locucion a camara)
('Comisión de Alpu', 'percentage', 0.04, 'alpu_commission'),							-- 04% de comisión para la organización Alpu 
('Comisión de Contador', 'percentage', 0.03, 'accountant_commission'); 					-- 03% de comisión para el contador de Alpu


-- =========================
-- CONTRACTS (EJEMPLOS)
-- =========================

INSERT INTO contract (client_id, contract_serial, broadcaster_id, date, due_date, country_code, total_price, term_years, total_price_post_tax) VALUES
(12, 'CON-001', 7, '2026-04-01', '2026-04-08', 'UY', 6600.00, 1, 7920.00),
(12, 'CON-002', 8, '2026-04-01', '2026-05-01', 'AR', 9600.00, 1, 11520.00),
(13, 'CON-003', 9, '2026-04-05', '2026-04-12', 'CL', 14700.00, 1, 17640.00),
(12, 'CON-004', 10, '2026-04-10', '2026-07-10', 'PY', 24500.00, 1, 29400.00),
(13, 'CON-005', 10, '2026-04-01', '2026-04-30', 'UY', 5100.00, 1, 6120.00);

-- =========================
-- CAMPAIGN
-- =========================

INSERT INTO campaign (contract_id, name) VALUES
(1, 'Campaign A'),
(2, 'Campaign B'),
(3, 'Campaign C'),
(3, 'Campaign C');

INSERT INTO campaign_service (campaign_id, service_id, base_price_override) VALUES
(1, 1, NULL),
(2, 3, NULL),
(2, 4, NULL),
(3, 7, NULL),
(3, 7, NULL);

-- =========================
-- PIECES (EJEMPLOS)
-- =========================

INSERT INTO piece (campaign_service_id, name) VALUES
(1, 'Cuña Radiofónica - Producto A'),
(2, 'Spot Radio - Campaña Primavera'),
(3, 'Publicidad TV - Producto B'),
(4, 'Video Corporativo'),
(5, 'Audio para Redes - Instagram');

-- =========================
-- BILLS (EJEMPLOS)
-- =========================

INSERT INTO bill (contract_id, type, title, description, date, amount, pdf_amazon_s3_key) VALUES
(1, 'income', 'Factura - Cuña Radiofónica', 'Pago por grabación de cuña radiofónica', '2026-04-08', 6600.00, 'factura_001.pdf'),
(2, 'income', 'Factura - Spot Radio Campaña', 'Pago por 3 spots radiofónicos', '2026-05-05', 9600.00, 'factura_002.pdf'),
(3, 'income', 'Factura - Publicidad TV', 'Pago por spot televisivo', '2026-04-12', 14700.00, 'factura_003.pdf'),
(4, 'income', 'Factura - Video Corporativo', 'Pago por video corporativo 1 semana', '2026-04-15', 24500.00, 'factura_004.pdf'),
(5, 'income', 'Factura - Audio Redes Sociales', 'Pago por contenido para redes', '2026-05-01', 6700.00, 'factura_005.pdf');

-- =========================
-- NOTIFICATIONS (EJEMPLOS)

-- =========================
INSERT INTO notification (user_id, title, description, date, is_read) VALUES
(1, 'Nuevo contrato asignado', 'Tienes un nuevo contrato para grabar', '2026-04-01 09:30:00', FALSE),
(2, 'Pago procesado', 'Tu pago ha sido procesado exitosamente', '2026-04-05 14:15:00', TRUE),
(3, 'Recordatorio de membresía', 'Tu membresía vence el 2026-04-01', '2026-03-28 10:00:00', TRUE),
(4, 'Nuevo trabajo disponible', 'Se requiere locutor para video corporativo', '2026-04-10 11:45:00', FALSE),
(5, 'Membresía vencida', 'Por favor renueva tu membresía', '2026-03-02 08:00:00', TRUE);

-- =========================
-- VALIDACIONES Y CONFIRMACIÓN
-- =========================

COMMIT;

SELECT 'Precarga completada exitosamente.' AS status;

-- Verificaciones de datos precargados:
SELECT 'RESUMEN DE DATOS PRECARGADOS:' AS info;
SELECT COUNT(*) as total_countries FROM country;
SELECT COUNT(*) as total_regions FROM region;
SELECT COUNT(*) as total_users FROM "user";
SELECT COUNT(*) as total_broadcasters FROM broadcaster;
SELECT COUNT(*) as total_clients FROM client;
SELECT COUNT(*) as total_media FROM service;
SELECT COUNT(*) as total_discounts FROM price_adjustment;
SELECT COUNT(*) as total_contracts FROM contract;
SELECT COUNT(*) as total_pieces FROM piece;
SELECT COUNT(*) as total_bills FROM bill;

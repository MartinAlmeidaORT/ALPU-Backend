BEGIN;

-- =============================================================
-- JUEGO DE PRUEBA EXTENDIDO: CONTRACTS + BILLS
-- Fecha de referencia: 2026-06-10
-- Incluye contratos atrasados (sin pagar >30 días), contratos
-- en todos los estados, y bills institucionales variadas.
-- =============================================================

-- =========================
-- ADDRESSES ADICIONALES
-- (para nuevos usuarios)
-- =========================

INSERT INTO address (country_code, department_id, city, street) VALUES
('UY', 10, 'Montevideo', 'Calle Paysandú 320'),       -- address_id 14
('UY', 10, 'Montevideo', 'Avenida Libertador 4500'),   -- address_id 15
('AR', 20, 'Buenos Aires', 'Corrientes 1234'),          -- address_id 16
('UY', 2,  'Canelones', 'Calle San Martín 800'),       -- address_id 17
('UY', 10, 'Montevideo', 'Calle Dr. Schroeder 1020'),  -- address_id 18
('CL', 59, 'Santiago', 'Av. Providencia 2550');        -- address_id 19

-- =========================
-- AGENCIAS ADICIONALES
-- =========================

INSERT INTO agency (name) VALUES
('Grupo Creativo del Sur'),      -- agency_id 3
('MediaPro Internacional'),      -- agency_id 4
('Voces del Plata SRL');         -- agency_id 5

-- =========================
-- USUARIOS CLIENTES ADICIONALES
-- =========================

INSERT INTO "user" (first_name, last_name, email, password, address_id, state, rut) VALUES
('Grupo', 'Creativo', 'grupocreativo@example.com',    '$2a$11$6M9rPx1gI4pBSVZygaXtfurHWD9oMnRlZEVHKVEGexKf6Gjm1xHca', 14, 'enabled', '11122233301A'),
('MediaPro', 'Internacional',  'mediapro@example.com', '$2a$11$6M9rPx1gI4pBSVZygaXtfurHWD9oMnRlZEVHKVEGexKf6Gjm1xHca', 15, 'enabled', '22233344402B'),
('Voces',   'del Plata',      'voces@example.com',    '$2a$11$6M9rPx1gI4pBSVZygaXtfurHWD9oMnRlZEVHKVEGexKf6Gjm1xHca', 16, 'enabled', '33344455503C');
-- user_id 14, 15, 16

INSERT INTO client (user_id, agency_id) VALUES
(14, 3),
(15, 4),
(16, 5);

-- =========================
-- USUARIOS LOCUTORES ADICIONALES
-- =========================

INSERT INTO "user" (first_name, last_name, email, password, address_id, state, rut) VALUES
('Sofía',   'Pérez',    'sofia.perez@alpu.com',    '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 17, 'enabled', '44455566604D'),
('Diego',   'Ramírez',  'diego.ramirez@alpu.com',  '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 18, 'enabled', '55566677705E'),
('Luciana', 'Torres',   'luciana.torres@alpu.com', '$2a$11$gQH8MNwJHJqCO5G2h/HQjOEVPF7vngx2HKCXhj.n/AMs3RBincSku', 19, 'enabled', '66677788806F');
-- user_id 17, 18, 19

INSERT INTO broadcaster (user_id, category_id) VALUES
(17, 1),  -- Principiante
(18, 2),  -- Profesional
(19, 3);  -- Master


INSERT INTO membership (broadcaster_id, pay_date, due_date, state, amount) VALUES
(17, '2026-05-01', '2027-05-01', 'valid',   500.00),
(18, '2026-04-01', '2027-04-01', 'valid',   500.00),
(19, '2025-12-01', '2026-12-01', 'expired', 500.00);

-- =============================================================
-- CONTRACTS EXTENDIDOS
-- Estado actual (2026-06-10):
--   Contratos 1-5: ya existentes en load-rework.sql
--   Contratos 6-20: nuevos
--
-- LEYENDA DE ESTADOS BUSCADOS:
--   'pending'   = generado, sin consentimiento completo o sin pago
--   'active'    = ambas partes aceptaron, aún vigente
--   'completed' = pagado y cerrado
--   'canceled'  = cancelado
--
-- ATRASADOS (para morosidad):
--   Contratos cuya due_date ya venció y siguen en 'active' sin bill de income.
--   Se marcan con comentario -- [ATRASADO]
-- =============================================================

INSERT INTO contract
    (client_id, contract_serial, broadcaster_id, state, client_approved, broadcaster_approved,
     date, due_date, country_code, total_price, term_years, pdf_amazon_s3_key, total_price_post_tax)
VALUES

-- -------------------------------------------------------
-- BLOQUE 1: CONTRATOS COMPLETADOS (pagados, cerrados)
-- -------------------------------------------------------

-- #6: Radio 1 mes, completado normalmente
(12, 'CON-006', 7,  'completed', TRUE,  TRUE,  '2026-01-05', '2026-02-05', 'UY', 9600.00,  1, NULL, 11520.00),

-- #7: TV 1 semana, completado con prueba de pago
(13, 'CON-007', 8,  'completed', TRUE,  TRUE,  '2026-01-10', '2026-01-17', 'AR', 14700.00, 1, 'comprobante_c7.pdf', 17640.00),

-- #8: Internet audio, completado
(14, 'CON-008', 9,  'completed', TRUE,  TRUE,  '2026-02-01', '2026-03-01', 'UY', 7400.00,  1, NULL, 8880.00),

-- #9: Narrativa, completado
(15, 'CON-009', 10, 'completed', TRUE,  TRUE,  '2026-02-15', '2026-03-15', 'CL', 6800.00,  1, 'comprobante_c9.pdf', 8160.00),

-- #10: IVR, completado
(16, 'CON-010', 11, 'completed', TRUE,  TRUE,  '2026-03-01', '2026-03-08', 'UY', 5300.00,  1, NULL, 6360.00),

-- #11: TV 3 meses, completado
(12, 'CON-011', 18, 'completed', TRUE,  TRUE,  '2026-03-10', '2026-06-10', 'UY', 24500.00, 1, 'comprobante_c11.pdf', 29400.00),

-- #12: Radio 1 mes, completado
(13, 'CON-012', 19, 'completed', TRUE,  TRUE,  '2026-03-20', '2026-04-20', 'AR', 9600.00,  1, NULL, 11520.00),

-- -------------------------------------------------------
-- BLOQUE 2: CONTRATOS ACTIVOS (firmados, dentro de plazo)
-- -------------------------------------------------------

-- #13: TV 1 mes, activo, vence 2026-07-10
(14, 'CON-013', 7,  'active', TRUE, TRUE, '2026-06-10', '2026-07-10', 'UY', 21200.00, 1, NULL, 25440.00),

-- #14: Radio 3 meses, activo, vence 2026-09-05
(15, 'CON-014', 8,  'active', TRUE, TRUE, '2026-06-05', '2026-09-05', 'CL', 11000.00, 1, NULL, 13200.00),

-- #15: Internet video 1 mes, activo, vence 2026-07-08
(16, 'CON-015', 17, 'active', TRUE, TRUE, '2026-06-08', '2026-07-08', 'MX', 14800.00, 1, NULL, 17760.00),

-- #16: Cine 1 semana, activo, vence 2026-06-17
(12, 'CON-016', 19, 'active', TRUE, TRUE, '2026-06-10', '2026-06-17', 'UY', 7300.00,  1, NULL, 8760.00),

-- -------------------------------------------------------
-- BLOQUE 3: CONTRATOS PENDIENTES (sin consentimiento completo)
-- -------------------------------------------------------

-- #17: Locutor aún no aprobó
(14, 'CON-017', 9,  'pending', TRUE,  FALSE, '2026-06-09', '2026-07-09', 'UY', 9600.00,  1, NULL, 11520.00),

-- #18: Cliente aún no aprobó
(15, 'CON-018', 10, 'pending', FALSE, TRUE,  '2026-06-08', '2026-07-08', 'AR', 6600.00,  1, NULL, 7920.00),

-- #19: Ninguno aprobó todavía
(16, 'CON-019', 11, 'pending', FALSE, FALSE, '2026-06-10', '2026-07-10', 'UY', 14700.00, 1, NULL, 17640.00),

-- -------------------------------------------------------
-- BLOQUE 4: CONTRATOS CANCELADOS
-- -------------------------------------------------------

-- #20: Cancelado por el cliente
(12, 'CON-020', 8,  'canceled', TRUE,  FALSE, '2026-04-20', '2026-05-20', 'UY', 9600.00,  1, NULL, 11520.00),

-- #21: Cancelado por el locutor
(13, 'CON-021', 17, 'canceled', FALSE, TRUE,  '2026-05-01', '2026-06-01', 'AR', 6800.00,  1, NULL, 8160.00),

-- #22: Cancelado después de firmado (reemplazado por #22A lógicamente)
(14, 'CON-022', 18, 'canceled', TRUE,  TRUE,  '2026-05-10', '2026-06-10', 'UY', 21200.00, 1, NULL, 25440.00),

-- -------------------------------------------------------
-- BLOQUE 5: CONTRATOS ATRASADOS (activos, vencidos, sin pago)
-- Estos deben ser marcados como morosos por la aplicación
-- (due_date ya pasó hace >30 días sin bill de income asociada)
-- -------------------------------------------------------

-- #23 [ATRASADO 66 días]: venció 2026-04-05 (65 días atrás)
(12, 'CON-023', 7,  'active', TRUE, TRUE, '2026-03-05', '2026-04-05', 'UY', 9600.00,  1, NULL, 11520.00),

-- #24 [ATRASADO 55 días]: venció 2026-04-16
(13, 'CON-024', 9,  'active', TRUE, TRUE, '2026-03-16', '2026-04-16', 'AR', 14700.00, 1, NULL, 17640.00),

-- #25 [ATRASADO 46 días]: venció 2026-04-25
(14, 'CON-025', 10, 'active', TRUE, TRUE, '2026-03-25', '2026-04-25', 'CL', 21200.00, 1, NULL, 25440.00),

-- #26 [ATRASADO 41 días]: venció 2026-04-30
(15, 'CON-026', 11, 'active', TRUE, TRUE, '2026-03-30', '2026-04-30', 'UY', 6600.00,  1, NULL, 7920.00),

-- #27 [ATRASADO 31 días]: venció 2026-05-10, justo en el límite
(16, 'CON-027', 18, 'active', TRUE, TRUE, '2026-04-10', '2026-05-10', 'UY', 11000.00, 1, NULL, 13200.00),

-- #28 [ATRASADO 47 días]: venció 2026-04-24, cliente reincidente
(12, 'CON-028', 8,  'active', TRUE, TRUE, '2026-03-24', '2026-04-24', 'AR', 6800.00,  1, NULL, 8160.00),

-- #29 [ATRASADO 36 días]: venció 2026-05-05
(13, 'CON-029', 19, 'active', TRUE, TRUE, '2026-04-05', '2026-05-05', 'UY', 24500.00, 1, NULL, 29400.00);

-- =============================================================
-- CAMPAIGNS para los nuevos contratos
-- El script original inserta 4 campañas (IDs 1-4), por lo tanto
-- las nuevas campañas reciben IDs 5-23.
-- =============================================================

INSERT INTO campaign (contract_id, name) VALUES
(6,  'Campaña Radio Enero'),      -- campaign_id 5
(7,  'Campaña TV Enero'),         -- campaign_id 6
(8,  'Campaña Internet Feb'),     -- campaign_id 7
(9,  'Narración Documental'),     -- campaign_id 8
(10, 'IVR Central'),              -- campaign_id 9
(11, 'Campaña TV Trimestral'),    -- campaign_id 10
(12, 'Campaña Radio Marzo'),      -- campaign_id 11
(13, 'Campaña TV Junio'),         -- campaign_id 12
(14, 'Radio Verano'),             -- campaign_id 13
(15, 'Redes Internacionales'),    -- campaign_id 14
(16, 'Spot Cine'),                -- campaign_id 15
(17, 'Radio Nueva'),              -- campaign_id 16
(23, 'Campaña Atrasada A'),       -- campaign_id 17
(24, 'Campaña Atrasada B'),       -- campaign_id 18
(25, 'Campaña Atrasada C'),       -- campaign_id 19
(26, 'Campaña Atrasada D'),       -- campaign_id 20
(27, 'Campaña Atrasada E'),       -- campaign_id 21
(28, 'Campaña Atrasada F'),       -- campaign_id 22
(29, 'Campaña Atrasada G');       -- campaign_id 23

-- campaign_service: el script original inserta 5 campaign_services (IDs 1-5),
-- por lo tanto los nuevos reciben IDs 6-24.
-- service 6 = RADIO, service 3 = TELEVISIÓN, service 10 = INTERNET AUDIO,
-- service 1 = NARRACIONES, service 2 = IVR, service 9 = INTERNET VIDEO, service 13 = CINE

INSERT INTO campaign_service (campaign_id, service_id) VALUES
(5,  6),   -- campaign_service_id 6  | Radio         (contrato 6)
(6,  3),   -- campaign_service_id 7  | TV             (contrato 7)
(7,  10),  -- campaign_service_id 8  | Internet audio (contrato 8)
(8,  1),   -- campaign_service_id 9  | Narración      (contrato 9)
(9,  2),   -- campaign_service_id 10 | IVR            (contrato 10)
(10, 3),   -- campaign_service_id 11 | TV             (contrato 11)
(11, 6),   -- campaign_service_id 12 | Radio          (contrato 12)
(12, 3),   -- campaign_service_id 13 | TV             (contrato 13)
(13, 6),   -- campaign_service_id 14 | Radio          (contrato 14)
(14, 9),   -- campaign_service_id 15 | Internet video (contrato 15)
(15, 13),  -- campaign_service_id 16 | Cine           (contrato 16)
(16, 6),   -- campaign_service_id 17 | Radio          (contrato 17)
(17, 3),   -- campaign_service_id 18 | TV  (atrasado A - contrato 23)
(18, 6),   -- campaign_service_id 19 | Radio (atrasado B - contrato 24)
(19, 3),   -- campaign_service_id 20 | TV  (atrasado C - contrato 25)
(20, 6),   -- campaign_service_id 21 | Radio (atrasado D - contrato 26)
(21, 3),   -- campaign_service_id 22 | TV  (atrasado E - contrato 27)
(22, 6),   -- campaign_service_id 23 | Radio (atrasado F - contrato 28)
(23, 3);   -- campaign_service_id 24 | TV  (atrasado G - contrato 29)

-- piece: el script original inserta 5 pieces (IDs 1-5),
-- los nuevos reciben IDs 6 en adelante.

INSERT INTO piece (campaign_service_id, name) VALUES
(6,  'Cuña Radio Enero'),
(7,  'Spot TV Enero'),
(8,  'Audio Instagram Feb'),
(9,  'Documental Corporativo'),
(10, 'Mensaje IVR Principal'),
(11, 'Spot TV Trimestral - 1'),
(11, 'Spot TV Trimestral - 2'),
(12, 'Cuña Radio Marzo'),
(13, 'Spot TV Junio A'),
(14, 'Cuña Radio Verano'),
(15, 'Video Redes Internacionales'),
(16, 'Trailer Cine');

-- =============================================================
-- BILLS (INGRESOS Y GASTOS)
-- =============================================================

-- -------------------------------------------------------
-- Pagos de contratos COMPLETADOS (income vinculado a contrato)
-- -------------------------------------------------------

INSERT INTO bill (contract_id, type, title, description, date, amount, pdf_amazon_s3_key) VALUES

-- Contratos 6-12 completados
(6,  'income', 'Pago - Radio Enero',          'Pago contrato radio 1 mes',         '2026-02-06', 9600.00,  'comprobante_b6.pdf'),
(7,  'income', 'Pago - TV Enero',             'Pago contrato televisión 1 semana',  '2026-01-18', 14700.00, 'comprobante_b7.pdf'),
(8,  'income', 'Pago - Internet Audio Feb',   'Pago contrato internet audio',       '2026-03-02', 7400.00,  'comprobante_b8.pdf'),
(9,  'income', 'Pago - Narración Documental', 'Pago narración documental',          '2026-03-16', 6800.00,  'comprobante_b9.pdf'),
(10, 'income', 'Pago - IVR Central',          'Pago contrato IVR',                  '2026-03-09', 5300.00,  'comprobante_b10.pdf'),
(11, 'income', 'Pago - TV Trimestral',        'Pago contrato TV 3 meses',           '2026-06-11', 24500.00, 'comprobante_b11.pdf'),
(12, 'income', 'Pago - Radio Marzo',          'Pago contrato radio 1 mes',          '2026-04-21', 9600.00,  'comprobante_b12.pdf'),

-- Pagos parciales de contratos activos (anticipo)
(13, 'income', 'Anticipo - TV Junio',         'Anticipo 50% contrato TV',           '2026-06-10', 10600.00, 'comprobante_b13_anticipo.pdf'),
(15, 'income', 'Anticipo - Redes Internac.',  'Anticipo contrato internet video',   '2026-06-08', 7400.00,  'comprobante_b15_anticipo.pdf'),

-- -------------------------------------------------------
-- GASTOS INSTITUCIONALES (sin contrato)
-- -------------------------------------------------------

-- Gastos fijos mensuales
(NULL, 'expense', 'Alquiler oficina - Enero',    'Alquiler mensual sede ALPU',              '2026-01-05', 18000.00, 'recibo_alquiler_ene.pdf'),
(NULL, 'expense', 'Alquiler oficina - Febrero',  'Alquiler mensual sede ALPU',              '2026-02-05', 18000.00, 'recibo_alquiler_feb.pdf'),
(NULL, 'expense', 'Alquiler oficina - Marzo',    'Alquiler mensual sede ALPU',              '2026-03-05', 18000.00, 'recibo_alquiler_mar.pdf'),
(NULL, 'expense', 'Alquiler oficina - Abril',    'Alquiler mensual sede ALPU',              '2026-04-05', 18000.00, 'recibo_alquiler_abr.pdf'),
(NULL, 'expense', 'Alquiler oficina - Mayo',     'Alquiler mensual sede ALPU',              '2026-05-05', 18000.00, 'recibo_alquiler_may.pdf'),
(NULL, 'expense', 'Alquiler oficina - Junio',    'Alquiler mensual sede ALPU',              '2026-06-05', 18000.00, 'recibo_alquiler_jun.pdf'),

-- Servicios y utilidades
(NULL, 'expense', 'UTE - Enero',                'Factura electricidad enero',               '2026-01-10', 3200.00,  'factura_ute_ene.pdf'),
(NULL, 'expense', 'UTE - Febrero',              'Factura electricidad febrero',             '2026-02-10', 3400.00,  'factura_ute_feb.pdf'),
(NULL, 'expense', 'UTE - Marzo',                'Factura electricidad marzo',               '2026-03-10', 3100.00,  'factura_ute_mar.pdf'),
(NULL, 'expense', 'UTE - Abril',                'Factura electricidad abril',               '2026-04-10', 3300.00,  'factura_ute_abr.pdf'),
(NULL, 'expense', 'UTE - Mayo',                 'Factura electricidad mayo',                '2026-05-10', 3250.00,  'factura_ute_may.pdf'),
(NULL, 'expense', 'ANTEL - Internet Abril',     'Servicio de internet y telefonía',         '2026-04-12', 1800.00,  'factura_antel_abr.pdf'),
(NULL, 'expense', 'ANTEL - Internet Mayo',      'Servicio de internet y telefonía',         '2026-05-12', 1800.00,  'factura_antel_may.pdf'),
(NULL, 'expense', 'ANTEL - Internet Junio',     'Servicio de internet y telefonía',         '2026-06-12', 1800.00,  'factura_antel_jun.pdf'),

-- Infraestructura y tecnología
(NULL, 'expense', 'Hosting AWS - Q1',           'Costo trimestral de hosting en AWS',       '2026-01-15', 12000.00, 'factura_aws_q1.pdf'),
(NULL, 'expense', 'Hosting AWS - Q2',           'Costo trimestral de hosting en AWS',       '2026-04-15', 12500.00, 'factura_aws_q2.pdf'),
(NULL, 'expense', 'Amazon S3 - Enero',          'Almacenamiento de demos y contratos',      '2026-01-31', 800.00,   'factura_s3_ene.pdf'),
(NULL, 'expense', 'Amazon S3 - Febrero',        'Almacenamiento de demos y contratos',      '2026-02-28', 850.00,   'factura_s3_feb.pdf'),
(NULL, 'expense', 'Amazon S3 - Marzo',          'Almacenamiento de demos y contratos',      '2026-03-31', 900.00,   'factura_s3_mar.pdf'),
(NULL, 'expense', 'Amazon S3 - Abril',          'Almacenamiento de demos y contratos',      '2026-04-30', 920.00,   'factura_s3_abr.pdf'),
(NULL, 'expense', 'Amazon S3 - Mayo',           'Almacenamiento de demos y contratos',      '2026-05-31', 950.00,   'factura_s3_may.pdf'),

-- Eventos y gastos extraordinarios
(NULL, 'expense', 'Evento Anual Locutores',     'Organización evento anual ALPU 2026',      '2026-03-20', 45000.00, 'recibo_evento_anual.pdf'),
(NULL, 'expense', 'Material de oficina Q1',     'Insumos de oficina primer trimestre',      '2026-03-28', 5600.00,  'ticket_oficina_q1.pdf'),
(NULL, 'expense', 'Material de oficina Q2',     'Insumos de oficina segundo trimestre',     '2026-06-02', 4800.00,  'ticket_oficina_q2.pdf'),
(NULL, 'expense', 'Asesoría legal Marzo',       'Consultoría jurídica contratos locutores', '2026-03-15', 9500.00,  'factura_legal_mar.pdf'),
(NULL, 'expense', 'Asesoría legal Mayo',        'Revisión reglamento interno ALPU',         '2026-05-18', 8000.00,  'factura_legal_may.pdf'),
(NULL, 'expense', 'Capacitación equipo',        'Taller de gestión digital para staff',     '2026-04-22', 7200.00,  'comprobante_cap.pdf'),
(NULL, 'expense', 'Diseño gráfico web',         'Rediseño de materiales digitales',         '2026-05-05', 15000.00, 'factura_disenio.pdf'),

-- Ingresos institucionales sin contrato (cuotas, donaciones, otros)
(NULL, 'income',  'Cuotas locutores - Enero',   'Recaudación mensual de membresías',        '2026-01-31', 12000.00, 'planilla_cuotas_ene.pdf'),
(NULL, 'income',  'Cuotas locutores - Febrero', 'Recaudación mensual de membresías',        '2026-02-28', 11500.00, 'planilla_cuotas_feb.pdf'),
(NULL, 'income',  'Cuotas locutores - Marzo',   'Recaudación mensual de membresías',        '2026-03-31', 12500.00, 'planilla_cuotas_mar.pdf'),
(NULL, 'income',  'Cuotas locutores - Abril',   'Recaudación mensual de membresías',        '2026-04-30', 11000.00, 'planilla_cuotas_abr.pdf'),
(NULL, 'income',  'Cuotas locutores - Mayo',    'Recaudación mensual de membresías',        '2026-05-31', 13000.00, 'planilla_cuotas_may.pdf'),
(NULL, 'income',  'Aporte extraordinario',      'Donación de locutor asociado honorario',   '2026-02-14', 5000.00,  'comprobante_donacion.pdf'),
(NULL, 'income',  'Subsidio MIEM',              'Subsidio institucional recibido',          '2026-03-10', 30000.00, 'comprobante_miem.pdf');

-- =============================================================
-- NOTIFICACIONES DE CONTRATOS ATRASADOS
-- (simula lo que el sistema generaría automáticamente)
-- =============================================================

INSERT INTO notification (user_id, title, description, date, is_read) VALUES
-- Notificación a clientes morosos
(12, 'Contrato vencido sin pago', 'El contrato #23 venció el 05/04/2026 y aún no registra pago.',  '2026-05-06 08:00:00', FALSE),
(12, 'Contrato vencido sin pago', 'El contrato #28 venció el 24/04/2026 y aún no registra pago.',  '2026-05-25 08:00:00', FALSE),
(13, 'Contrato vencido sin pago', 'El contrato #24 venció el 16/04/2026 y aún no registra pago.',  '2026-05-17 08:00:00', FALSE),
(13, 'Contrato vencido sin pago', 'El contrato #29 venció el 05/05/2026 y aún no registra pago.',  '2026-06-05 08:00:00', FALSE),
(14, 'Contrato vencido sin pago', 'El contrato #25 venció el 25/04/2026 y aún no registra pago.',  '2026-05-26 08:00:00', FALSE),
(15, 'Contrato vencido sin pago', 'El contrato #26 venció el 30/04/2026 y aún no registra pago.',  '2026-05-31 08:00:00', FALSE),
(16, 'Contrato vencido sin pago', 'El contrato #27 venció el 10/05/2026 y aún no registra pago.',  '2026-06-10 08:00:00', FALSE),
-- Notificación a locutores afectados
(7,  'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #23 dentro del plazo.', '2026-05-06 08:05:00', FALSE),
(8,  'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #28 dentro del plazo.', '2026-05-25 08:05:00', FALSE),
(9,  'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #24 dentro del plazo.', '2026-05-17 08:05:00', FALSE),
(10, 'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #25 dentro del plazo.', '2026-05-26 08:05:00', FALSE),
(11, 'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #26 dentro del plazo.', '2026-05-31 08:05:00', FALSE),
(18, 'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #27 dentro del plazo.', '2026-06-10 08:05:00', FALSE),
(19, 'Pago pendiente de cliente', 'El cliente no realizó el pago del contrato #29 dentro del plazo.', '2026-06-05 08:05:00', FALSE);

-- =============================================================
-- RESUMEN DEL JUEGO DE PRUEBA AGREGADO
-- =============================================================
-- Contratos nuevos (6-29): 24
--   Completados:  7  (#6 al #12)
--   Activos:      4  (#13 al #16)
--   Pendientes:   3  (#17 al #19)
--   Cancelados:   3  (#20 al #22)
--   ATRASADOS:    7  (#23 al #29)  ← clientes 12, 13, 14, 15, 16
--
-- Bills nuevas:
--   Ingresos por contratos:  9
--   Gastos institucionales: 27
--   Ingresos institucionales: 8
--   Total nuevas bills:     44
--
-- Clientes morosos: 12, 13, 14, 15, 16 (todos tienen ≥1 contrato atrasado)
-- Cliente 12 tiene 2 contratos atrasados (#23 y #28)
-- Cliente 13 tiene 2 contratos atrasados (#24 y #29)
-- =============================================================

COMMIT;

-- =========================
-- ENUMS
-- =========================

CREATE TYPE price_adjustment_type_enum AS ENUM (
    'fixed',
    'percentage'
);

CREATE TYPE user_state_enum AS ENUM (
    'enabled',
    'pending',
    'penalized'
);

CREATE TYPE membership_state_enum AS ENUM (
    'valid',
    'expired'
);

CREATE TYPE bill_type_enum AS ENUM (
    'expense',
    'income'
);

CREATE TYPE service_type_enum AS ENUM (
	'tv_generic',
	'tv_zocalo',
	'tv_host',
	'radio_generic',
	'radio_zocalo',
	'radio_host',
	'internet_video',
	'internet_audio',
	'others_video',
	'others_audio',
	'others',
	'cinema',
	'ivr',
	'narrative',
	'camera',
	'event'
);

CREATE TYPE interval_enum AS ENUM (
	'one_week',
	'one_month',
	'three_months',
    'six_months',
    'one_year'
);

CREATE TYPE contract_state_enum AS ENUM (
    'pending',
	'active',
	'completed',
    'canceled',
    'paid'
);

CREATE TYPE gender_enum AS ENUM (
    'male',
    'female',
    'non_specified'
);

-- =========================
-- COUNTRY & REGION
-- =========================

CREATE TABLE region (
    region_id SERIAL PRIMARY KEY,
    multiplier NUMERIC NOT NULL CHECK (multiplier > 0)
);

CREATE TABLE country (
    country_code char(3) PRIMARY KEY,
    region_id INT REFERENCES region(region_id),
    name VARCHAR(50) NOT NULL
);

CREATE TABLE department (
	department_id SERIAL PRIMARY KEY,
	name VARCHAR(100) NOT NULL,
	country_code char(3) NOT NULL REFERENCES country(country_code)
);

-- =========================
-- ADDRESS
-- =========================

CREATE TABLE address (
    address_id SERIAL PRIMARY KEY,
    country_code char(3) NOT NULL REFERENCES country(country_code),
    department_id INT REFERENCES department(department_id),
    city VARCHAR(100) NOT NULL,
    street VARCHAR(100)
);

-- =========================
-- BROADCASTER CATEGORY
-- =========================

CREATE TABLE broadcaster_category (
    broadcaster_category_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    lifetime_job_count INT NOT NULL CHECK(lifetime_job_count >= 0)
);

-- =========================
-- AGENCY
-- =========================

CREATE TABLE agency (
	agency_id SERIAL PRIMARY KEY,
	name VARCHAR(100) NOT NULL
);

CREATE TABLE skill (
    skill_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE "language" (
    language_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- =========================
-- USER (BASE)
-- =========================

CREATE TABLE "user" (
    user_id SERIAL PRIMARY KEY,
	google_id VARCHAR(25) UNIQUE,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(60),
    first_name VARCHAR(50) NOT NULL CHECK (LENGTH(first_name) > 2),
    last_name VARCHAR(50) NOT NULL CHECK (LENGTH(last_name) > 2),
    rut VARCHAR(12) UNIQUE NOT NULL,
    address_id INT NOT NULL UNIQUE REFERENCES address(address_id),
    photo_amazon_s3_key VARCHAR(200),
    gender gender_enum,
    identity_card VARCHAR(8),
    state user_state_enum DEFAULT 'pending',
	CONSTRAINT chk_password_or_google
	CHECK (
	    (google_id IS NULL AND password IS NOT NULL) OR
	    (google_id IS NOT NULL AND password IS NULL)
	)
);


-- =========================
-- CLIENT / BROADCASTER / ADMINISTRATOR / SUPERVISOR / ACCOUNTANT (HERENCIA)
-- =========================

CREATE TABLE client (
    user_id INT PRIMARY KEY REFERENCES "user"(user_id) ON DELETE CASCADE,
	agency_id INT NOT NULL REFERENCES agency(agency_id)
);

CREATE TABLE broadcaster (
    user_id INT PRIMARY KEY REFERENCES "user"(user_id) ON DELETE CASCADE,
    category_id INT NOT NULL REFERENCES broadcaster_category(broadcaster_category_id),
    phone_number VARCHAR(20),
    website VARCHAR(200),
    description VARCHAR(1000)
);

CREATE TABLE administrator (
	user_id INT PRIMARY KEY REFERENCES "user"(user_id) ON DELETE CASCADE
);

CREATE TABLE supervisor (
	user_id INT PRIMARY KEY REFERENCES "user"(user_id) ON DELETE CASCADE
);

CREATE TABLE accountant (
	user_id INT PRIMARY KEY REFERENCES "user"(user_id) ON DELETE CASCADE
);

-- =========================
-- DEMO
-- =========================

CREATE TABLE demo (
    broadcaster_id INT REFERENCES broadcaster(user_id),
    file_name VARCHAR(200) NOT NULL,
    language_id INT NOT NULL REFERENCES language(language_id),
    title VARCHAR(200) NOT NULL,
    PRIMARY KEY (broadcaster_id, file_name)
);

-- =========================
-- BROADCASTER / SKILL / LANGUAGE
-- =========================

create table broadcaster_skills (
    broadcaster_id INT NOT NULL REFERENCES broadcaster(user_id),
    skill_id INT NOT NULL REFERENCES skill(skill_id),
    PRIMARY KEY (broadcaster_id, skill_id)
);

create table broadcaster_languages (
    broadcaster_id INT NOT NULL REFERENCES broadcaster(user_id),
    language_id INT NOT NULL REFERENCES language(language_id),
    PRIMARY KEY (broadcaster_id, language_id)
);

-- =========================
-- NOTIFICATION
-- =========================

CREATE TABLE notification (
    notification_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES "user"(user_id),
    title VARCHAR(200) NOT NULL,
    description VARCHAR(500) NOT NULL,
    date TIMESTAMP NOT NULL,
    is_read BOOLEAN DEFAULT FALSE
);


-- =========================
-- MEMBERSHIP
-- =========================

CREATE TABLE membership (
    membership_id SERIAL PRIMARY KEY,
    broadcaster_id INT NOT NULL REFERENCES broadcaster(user_id),
    pay_date DATE,
    due_date DATE,
    state membership_state_enum DEFAULT 'expired',
    amount NUMERIC CHECK (amount > 0)
);

-- =========================
-- PRICE_ADJUSTMENT
-- =========================

CREATE TABLE price_adjustment (
    price_adjustment_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
	type price_adjustment_type_enum NOT NULL,
    amount NUMERIC NOT NULL CHECK (amount != 0),
	key VARCHAR(100) NOT NULL
);

-- =========================
-- CONTRACT
-- =========================

CREATE TABLE contract (
    contract_id SERIAL PRIMARY KEY,
    contract_serial VARCHAR(50) UNIQUE,
    root_contract_id INT REFERENCES contract(contract_id),
    replaces_contract_id INT REFERENCES contract(contract_id),
    client_id INT NOT NULL REFERENCES client(user_id),
	client_approved BOOL NOT NULL DEFAULT FALSE,
    broadcaster_id INT NOT NULL REFERENCES broadcaster(user_id),
	broadcaster_approved BOOL NOT NULL DEFAULT FALSE,
	state contract_state_enum NOT NULL DEFAULT 'pending',
    date DATE NOT NULL,
    due_date DATE NOT NULL,
    country_code char(3) NOT NULL REFERENCES country(country_code),
    total_price NUMERIC NOT NULL CHECK (total_price > 0),
    term_years INT NOT NULL,
    pdf_amazon_s3_key VARCHAR(200),
    total_price_post_tax NUMERIC NOT NULL CHECK (total_price_post_tax > 0)
);

-- =========================
-- BILL
-- =========================

CREATE TABLE bill (
    bill_id SERIAL PRIMARY KEY,
    contract_id INT REFERENCES contract(contract_id),
    type bill_type_enum NOT NULL,
    title VARCHAR(200) NOT NULL,
    description VARCHAR(500) NOT NULL ,
    date DATE NOT NULL,
    amount NUMERIC NOT NULL CHECK (amount != 0),
    pdf_amazon_s3_key VARCHAR(200) NOT NULL
);

-- =========================
-- SERVICE
-- =========================

CREATE TABLE service (
    service_id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
	discriminator VARCHAR(100) NOT NULL,
	type service_type_enum NOT NULL,
	base_price NUMERIC CHECK (base_price > 0),
	extra_price NUMERIC CHECK (extra_price > 0),
	first_extra_price NUMERIC CHECK (first_extra_price > 0),
	update_message_price NUMERIC CHECK (update_message_price > 0),
	role_price NUMERIC CHECK (role_price > 0)
);

-- =========================
-- SERVICE EXTRA
-- =========================

CREATE TABLE service_period (
    service_id INT REFERENCES service(service_id),
	base_price NUMERIC NOT NULL CHECK (extra_price > 0),
	extra_price NUMERIC CHECK (extra_price > 0),
	first_extra_price NUMERIC CHECK (first_extra_price > 0),
	interval interval_enum NOT NULL,
    PRIMARY KEY (service_id, interval)
);

CREATE TABLE range_ivr (
    service_id INT REFERENCES service(service_id),
    min_word INT NOT NULL CHECK(min_word >= 0),
    max_word INT CHECK (max_word >= 0),
    price_per_word NUMERIC NOT NULL,
    PRIMARY KEY (service_id, min_word)
);

-- =========================
-- VOLUME DISCOUNT
-- =========================

CREATE TABLE volume_discount (
    volume_discount_id SERIAL PRIMARY KEY,
	name VARCHAR(100) NOT NULL,
	service_type service_type_enum NOT NULL,
    min_quantity INT NOT NULL,
	max_quantity INT,
	type price_adjustment_type_enum NOT NULL,
    amount NUMERIC NOT NULL CHECK (amount > 0)
);

-- =========================
-- MULTI SERVICE DISCOUNT
-- =========================

CREATE TABLE multi_service_discount (
    mult_service_discount_id SERIAL PRIMARY KEY,
	name VARCHAR(100) NOT NULL,
	key VARCHAR(100) NOT NULL,
	service_a service_type_enum NOT NULL,
	service_b service_type_enum NOT NULL,
	type price_adjustment_type_enum NOT NULL,
    amount NUMERIC NOT NULL CHECK (amount > 0),
	is_discount_for_service_b_only BOOL NOT NULL DEFAULT FALSE
);

-- =========================
-- CAMPAIN
-- =========================

CREATE TABLE campaign (
    campaign_id SERIAL PRIMARY KEY,
    contract_id INT NOT NULL REFERENCES contract(contract_id),
    name VARCHAR(100) NOT NULL
);

CREATE TABLE campaign_service (
    campaign_service_id SERIAL PRIMARY KEY,
	campaign_id INT NOT NULL REFERENCES campaign(campaign_id),
    service_id INT NOT NULL REFERENCES service(service_id),
	base_price_override NUMERIC CHECK (base_price_override > 0)
);

CREATE TABLE campaign_service_date (
	campaign_service_id INT REFERENCES campaign_service(campaign_service_id),
	date DATE,
	PRIMARY KEY (campaign_service_id, date)
);

-- =========================
-- PIECE
-- =========================

CREATE TABLE piece (
    piece_id SERIAL PRIMARY KEY,
    campaign_service_id INT NOT NULL REFERENCES campaign_service(campaign_service_id),
    name VARCHAR(200) NOT NULL,
	UNIQUE (campaign_service_id, name)
);

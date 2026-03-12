-- ============================================================
-- PRINT CONFIGURATION - DATABASE INSERT SCRIPTS
-- Generated from setPrint.json
-- ============================================================

-- NOTE: Your tables are missing primary keys and sequences!
-- You should add these first:
-- 
-- ALTER TABLE FORMS ADD CONSTRAINT PK_FORMS PRIMARY KEY (ID);
-- ALTER TABLE FORM_PATTERNS ADD (ID NUMBER, CONSTRAINT PK_FORM_PATTERNS PRIMARY KEY (ID));
-- ALTER TABLE PATTERN_FIELDS ADD (ID NUMBER, CONSTRAINT PK_PATTERN_FIELDS PRIMARY KEY (ID));
-- 
-- CREATE SEQUENCE FORMS_SEQ START WITH 1;
-- CREATE SEQUENCE FORM_PATTERNS_SEQ START WITH 1;
-- CREATE SEQUENCE PATTERN_FIELDS_SEQ START WITH 1;

-- ============================================================
-- INSERT FORMS
-- ============================================================

-- Form Code 168: Sales Invoice
INSERT INTO FORMS (ID, COMPANY_CODE, FORM_CODE, FORM_EDESC, FORM_TYPE)
VALUES (1, '01', '168', 'Sales Invoice', 'OT');

-- Form Code 400: Sales Invoice
INSERT INTO FORMS (ID, COMPANY_CODE, FORM_CODE, FORM_EDESC, FORM_TYPE)
VALUES (2, '01', '400', 'Sales Invoice', 'OT');

-- ============================================================
-- INSERT FORM_PATTERNS
-- ============================================================

-- Pattern 1 for Form 168
INSERT INTO FORM_PATTERNS (
    FORM_ID, PATTERN_KEY, VIEW_FILE, FORM_TYPE, AUTO, CHARGE_EXIST,
    COMPANY_NAME, COMPANY_VAT_NO, COMPANY_ADDRESS, COMPANY_PHONE, COMPANY_EMAIL,
    INVOICE_QUERY
) VALUES (
    1, -- FORM_ID references FORMS.ID for form_code 168
    'pattern1',
    'pattern1.html',
    'OT',
    0, -- AUTO = false
    1, -- CHARGE_EXIST = true
    'First Company',
    '600123456',
    'Kathmandu, Nepal',
    '+977-1-4444444',
    'info@abctrading.com',
    'SELECT sales_date, sales_miti, sales_no, REGD_OFFICE_EADDRESS, customer_edesc, TPIN_VAT_NO, TEL_MOBILE_NO1, serial_no, item_edesc, mu_code, unit_price, quantity, total_price FROM VPRINT_SA_SALES_INVOICE WHERE SALES_NO = :salesNo AND company_code = :companyCode AND form_code = :formCode ORDER BY serial_no'
);

-- Pattern 2 for Form 168
INSERT INTO FORM_PATTERNS (
    FORM_ID, PATTERN_KEY, VIEW_FILE, FORM_TYPE, AUTO, CHARGE_EXIST,
    COMPANY_NAME, COMPANY_VAT_NO, COMPANY_ADDRESS, COMPANY_PHONE, COMPANY_EMAIL,
    INVOICE_QUERY
) VALUES (
    1, -- FORM_ID references FORMS.ID for form_code 168
    'pattern2',
    'pattern2.html',
    'OT',
    1, -- AUTO = true
    1, -- CHARGE_EXIST = true
    'First Company',
    '600123456',
    'Kathmandu, Nepal',
    '+977-1-4444444',
    'info@abctrading.com',
    'SELECT sales_date, sales_miti, sales_no, REGD_OFFICE_EADDRESS, customer_edesc, TPIN_VAT_NO, TEL_MOBILE_NO1, serial_no, item_edesc, mu_code, unit_price, quantity, total_price FROM VPRINT_SA_SALES_INVOICE WHERE SALES_NO = :salesNo AND company_code = :companyCode AND form_code = :formCode ORDER BY serial_no'
);

-- Pattern 1 for Form 400
INSERT INTO FORM_PATTERNS (
    FORM_ID, PATTERN_KEY, VIEW_FILE, FORM_TYPE, AUTO, CHARGE_EXIST,
    COMPANY_NAME, COMPANY_VAT_NO, COMPANY_ADDRESS, COMPANY_PHONE, COMPANY_EMAIL,
    INVOICE_QUERY
) VALUES (
    2, -- FORM_ID references FORMS.ID for form_code 400
    'pattern1',
    'pattern1.html',
    'OT',
    0, -- AUTO = false
    1, -- CHARGE_EXIST = true
    'First Company',
    '600123456',
    'Kathmandu, Nepal',
    '+977-1-4444444',
    'info@abctrading.com',
    'SELECT sales_date, sales_miti, sales_no, REGD_OFFICE_EADDRESS, customer_edesc, TPIN_VAT_NO, TEL_MOBILE_NO1, serial_no, item_edesc, mu_code, unit_price, quantity, total_price FROM VPRINT_SA_SALES_INVOICE WHERE SALES_NO = :salesNo AND company_code = :companyCode AND form_code = :formCode ORDER BY serial_no'
);

-- Pattern 2 for Form 400
INSERT INTO FORM_PATTERNS (
    FORM_ID, PATTERN_KEY, VIEW_FILE, FORM_TYPE, AUTO, CHARGE_EXIST,
    COMPANY_NAME, COMPANY_VAT_NO, COMPANY_ADDRESS, COMPANY_PHONE, COMPANY_EMAIL,
    INVOICE_QUERY
) VALUES (
    2, -- FORM_ID references FORMS.ID for form_code 400
    'pattern2',
    'pattern2.html',
    'OT',
    0, -- AUTO = false
    1, -- CHARGE_EXIST = true
    'First Company',
    '600123456',
    'Kathmandu, Nepal',
    '+977-1-4444444',
    'info@abctrading.com',
    'SELECT sales_date, sales_miti, sales_no, REGD_OFFICE_EADDRESS, customer_edesc, TPIN_VAT_NO, TEL_MOBILE_NO1, serial_no, item_edesc, mu_code, unit_price, quantity, total_price FROM VPRINT_SA_SALES_INVOICE WHERE SALES_NO = :salesNo AND company_code = :companyCode AND form_code = :formCode ORDER BY serial_no'
);

-- ============================================================
-- INSERT PATTERN_FIELDS
-- Note: PATTERN_ID should reference FORM_PATTERNS.ID
-- Assuming Pattern IDs will be auto-generated, adjust as needed
-- ============================================================

-- Pattern 1 (Form 168) - Master Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'sales_date', 'Date');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'sales_miti', 'Miti');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'sales_no', 'Invoice No');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'customer_edesc', 'Customer');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'REGD_OFFICE_EADDRESS', 'Address');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'TPIN_VAT_NO', 'VAT/PAN');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'MASTER', 'TEL_MOBILE_NO1', 'Phone');

-- Pattern 1 (Form 168) - Child Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'CHILD', 'serial_no', 'S.N.');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'CHILD', 'item_edesc', 'Description');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'CHILD', 'mu_code', 'Unit');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'CHILD', 'quantity', 'Quantity');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'CHILD', 'unit_price', 'Rate');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (1, 'CHILD', 'total_price', 'Amount');

-- Pattern 2 (Form 168) - Master Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'sales_date', 'Date');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'sales_miti', 'Miti');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'sales_no', 'Invoice No');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'customer_edesc', 'Customer');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'REGD_OFFICE_EADDRESS', 'Address');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'TPIN_VAT_NO', 'VAT/PAN');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'MASTER', 'TEL_MOBILE_NO1', 'Phone');

-- Pattern 2 (Form 168) - Child Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'CHILD', 'serial_no', 'S.N.');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'CHILD', 'item_edesc', 'Description');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'CHILD', 'mu_code', 'Unit');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'CHILD', 'quantity', 'Quantity');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'CHILD', 'unit_price', 'Rate');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (2, 'CHILD', 'total_price', 'Amount');

-- Pattern 3 (Form 400) - Master Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'sales_date', 'Date');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'sales_miti', 'Miti');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'sales_no', 'Invoice No');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'customer_edesc', 'Customer');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'REGD_OFFICE_EADDRESS', 'Address');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'TPIN_VAT_NO', 'VAT/PAN');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'MASTER', 'TEL_MOBILE_NO1', 'Phone');

-- Pattern 3 (Form 400) - Child Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'CHILD', 'serial_no', 'S.N.');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'CHILD', 'item_edesc', 'Description');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'CHILD', 'mu_code', 'Unit');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'CHILD', 'quantity', 'Quantity');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'CHILD', 'unit_price', 'Rate');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (3, 'CHILD', 'total_price', 'Amount');

-- Pattern 4 (Form 400) - Master Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'sales_date', 'Date');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'sales_miti', 'Miti');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'sales_no', 'Invoice No');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'customer_edesc', 'Customer');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'REGD_OFFICE_EADDRESS', 'Address');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'TPIN_VAT_NO', 'VAT/PAN');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'MASTER', 'TEL_MOBILE_NO1', 'Phone');

-- Pattern 4 (Form 400) - Child Fields
INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'CHILD', 'serial_no', 'S.N.');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'CHILD', 'item_edesc', 'Description');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'CHILD', 'mu_code', 'Unit');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'CHILD', 'quantity', 'Quantity');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'CHILD', 'unit_price', 'Rate');

INSERT INTO PATTERN_FIELDS (PATTERN_ID, FIELD_TYPE, FIELD_NAME, LABEL) 
VALUES (4, 'CHILD', 'total_price', 'Amount');

COMMIT;

-- ============================================================
-- VERIFICATION QUERIES
-- ============================================================

-- Check inserted data
SELECT * FROM FORMS;
SELECT * FROM FORM_PATTERNS;
SELECT * FROM PATTERN_FIELDS ORDER BY PATTERN_ID, FIELD_TYPE, FIELD_NAME;

-- Count records
SELECT 'FORMS' AS TABLE_NAME, COUNT(*) AS RECORD_COUNT FROM FORMS
UNION ALL
SELECT 'FORM_PATTERNS', COUNT(*) FROM FORM_PATTERNS
UNION ALL
SELECT 'PATTERN_FIELDS', COUNT(*) FROM PATTERN_FIELDS;

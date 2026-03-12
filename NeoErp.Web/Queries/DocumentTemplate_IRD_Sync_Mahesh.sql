
/*
-----------------------------------------------------------------------------------
 Database Scripts: IRD SYNC Related Tables
 - Includes Tables, Views, newly added Columns, and Functions.


-----------------------------------------------------------------------------------
*/

BEGIN
   EXECUTE IMMEDIATE '
      CREATE TABLE IRD_LOG (
          VOUCHER_NO   VARCHAR2(100 BYTE) NOT NULL,
          MESSAGE      VARCHAR2(100 BYTE) NOT NULL,
          FORM_CODE    VARCHAR2(10 BYTE)  NOT NULL,
          CREATED_DATE DATE
      )';
EXCEPTION
   WHEN OTHERS THEN
      IF SQLCODE != -955 THEN -- ORA-00955: name already used
         RAISE;
      END IF;
END;


ALTER TABLE IRD_LOG
ADD (
    REQUEST_JSON   CLOB,
    RESPONSE_JSON  CLOB,
    AMOUNT         NUMBER(18,2),
    REQUEST_BY     VARCHAR2(100 BYTE)
);

ALTER TABLE IRD_LOG
ADD (
  TAX_AMOUNT NUMBER(18,2),
  VAT        NUMBER(18,2)
);

-- Replace values using actual credentials for Live (Production) environment
-- Current value for test purpose
INSERT INTO API_SETTING
(
  USER_NAME,
  API_PWD,
  SALES_URL,
  SALES_RETURN_URL,
  COMPANY_CODE,
  CREATED_BY,
  CREATED_DATE,
  DELETED_FLAG,
  PAN_NO,
  SERVER_NAME
)
VALUES
(
  'Test_CBMS',
  'test@321',
  'https://cbapi.ird.gov.np/api/bill',
  'https://cbapi.ird.gov.np/api/billreturn',
  '01',
  'ADMIN',
  SYSDATE,
  'N',
  '999999999',
  ''
);
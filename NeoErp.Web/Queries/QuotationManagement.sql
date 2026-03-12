DECLARE
   v_cnt   NUMBER;
BEGIN
   /* ============================================================
      SCRIPT LOG TABLE
      ============================================================ */
   BEGIN
      EXECUTE IMMEDIATE '
      CREATE TABLE DB_SCRIPT_LOG (
        SCRIPT_NAME VARCHAR2(100) PRIMARY KEY,
        EXECUTED_ON DATE
      )
    ';
   EXCEPTION
      WHEN OTHERS
      THEN
         IF SQLCODE != -955
         THEN
            RAISE;
         END IF;
   END;

   /* ============================================================
      CHECK SCRIPT EXECUTION
      ============================================================ */
   BEGIN
      EXECUTE IMMEDIATE '
      SELECT COUNT(1)
      FROM DB_SCRIPT_LOG
      WHERE SCRIPT_NAME = :1
    '
         INTO v_cnt
         USING 'QUOTATION_MODULE_V1';
   EXCEPTION
      WHEN OTHERS
      THEN
         v_cnt := 0;
   END;

   IF v_cnt = 0
   THEN
      /* ============================================================
         TABLES
         ============================================================ */

      -- SA_QUOTATION_SETUP
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TABLES
       WHERE TABLE_NAME = 'SA_QUOTATION_SETUP';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            q'[
      CREATE TABLE SA_QUOTATION_SETUP (
        TENDER_NO VARCHAR2(255),
        VALID_DATE DATE,
        ISSUE_DATE DATE,
        CREATED_DATE DATE,
        CREATED_BY VARCHAR2(100),
        COMPANY_CODE VARCHAR2(50),
        STATUS CHAR(1),
        REMARKS VARCHAR2(255),
        ID NUMBER,
        APPROVED_STATUS VARCHAR2(50),
        MODIFIED_DATE DATE,
        MODIFIED_BY VARCHAR2(100),
        BRANCH_CODE VARCHAR2(30),
        MANUAL_NO VARCHAR2(20),
        APPROVED_BY VARCHAR2(100),
        LOCAL_FLAG CHAR(1) DEFAULT 'Y' NOT NULL,
        FORM_CODE VARCHAR2(10),
        VAT_INCLUDE_FLAG CHAR(1) DEFAULT 'Y' NOT NULL
      )
    ]';
      END IF;

      -- SA_QUOTATION_ITEMS
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TABLES
       WHERE TABLE_NAME = 'SA_QUOTATION_ITEMS';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE q'[
      CREATE TABLE SA_QUOTATION_ITEMS (
        ID NUMBER,
        TENDER_NO VARCHAR2(255),
        ITEM_CODE VARCHAR2(100),
        SPECIFICATION VARCHAR2(255),
        IMAGE VARCHAR2(255),
        UNIT VARCHAR2(50),
        QUANTITY NUMBER,
        CATEGORY VARCHAR2(100),
        BRAND_NAME VARCHAR2(100),
        INTERFACE VARCHAR2(100),
        TYPE VARCHAR2(100),
        LAMINATION VARCHAR2(100),
        ITEM_SIZE VARCHAR2(100),
        THICKNESS VARCHAR2(100),
        COLOR VARCHAR2(100),
        GRADE VARCHAR2(100),
        SIZE_LENGTH NUMBER,
        SIZE_WIDTH NUMBER,
        DELETED_FLAG CHAR(1),
        REMARKS VARCHAR2(255),
        QUOTATION_NO NUMBER,
        FORM_CODE VARCHAR2(10)
      )
    ]';
      END IF;

      -- QUOTATION_BACK_LOGS
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TABLES
       WHERE TABLE_NAME = 'QUOTATION_BACK_LOG';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE q'[
      CREATE TABLE QUOTATION_BACK_LOG
(
ID             NUMBER PRIMARY KEY,
QUOTATION_ID   NUMBER,
QUOTATION_NO   NUMBER,
TENDER_NO      VARCHAR2(150),
TYPE           VARCHAR2 (70),
ACTION         VARCHAR2 (30),
ACTION_BY      VARCHAR2 (50),
ACTION_DATE    DATE,
CHANGED        VARCHAR2 (150),
REMARKS        VARCHAR2 (500),
COMPANY_CODE   VARCHAR2(30),
CONSTRAINT CHK_TYPE CHECK(TYPE IN ('ROLLBACK', 'REMARKS', 'CLONE', 'MODIFY')),
CONSTRAINT CHK_ACTION CHECK(ACTION IN ('APPROVAL', 'CHECKED', 'VERIFY', 'RECOMMENDED', 'APPROVED', 'REJECTED', 'ROLLBACK', 'CLONE', 'MODIFY', 'ADD'))
);
    ]';
      END IF;

      /* ============================================================
         SEQUENCE
         ============================================================ */
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_SEQUENCES
       WHERE SEQUENCE_NAME = 'QUOTATION_BACK_LOG_SEQ';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE '
      CREATE SEQUENCE QUOTATION_BACK_LOG_SEQ
      START WITH 1 INCREMENT BY 1 NOCACHE NOCYCLE
    ';
      END IF;

      /* ============================================================
         FUNCTION (CREATE OR REPLACE IS SAFE)
         ============================================================ */
      EXECUTE IMMEDIATE
         q'[
    CREATE OR REPLACE FUNCTION generate_tender_no(
      p_company_code VARCHAR2,
      p_form_code VARCHAR2
    ) RETURN VARCHAR2 IS
      l_prefix VARCHAR2(50);
      l_suffix VARCHAR2(50);
      l_body_length NUMBER;
      l_next_val NUMBER;
      l_body VARCHAR2(50);
      l_tender_no VARCHAR2(150);
    BEGIN
      SELECT CUSTOM_PREFIX_TEXT, CUSTOM_SUFFIX_TEXT, BODY_LENGTH
      INTO l_prefix, l_suffix, l_body_length
      FROM form_setup
      WHERE COMPANY_CODE = p_company_code
        AND deleted_flag = 'N'
        AND QUOTATION_FLAG = 'Y'
        AND form_code = p_form_code
        AND ROWNUM = 1;

      SELECT NVL(MAX(TO_NUMBER(REGEXP_SUBSTR(
        tender_no,
        l_prefix || '([0-9]+)' || l_suffix,
        1, 1, NULL, 1))), 0) + 1
      INTO l_next_val
      FROM sa_quotation_setup
      WHERE COMPANY_CODE = p_company_code
        AND status = 'E'
        AND REGEXP_LIKE(tender_no, l_prefix || '\d+' || l_suffix);

      IF LENGTH(l_next_val) > l_body_length THEN
        RETURN '';
      END IF;

      l_body := LPAD(l_next_val, l_body_length, '0');
      RETURN l_prefix || l_body || l_suffix;
    END;
  ]';

      /* ============================================================
         SAFE COLUMN ADD TEMPLATE (reused many times)
         ============================================================ */

      -- Example: quotation_details.checked_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'QUOTATION_DETAILS' AND COLUMN_NAME = 'CHECKED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD CHECKED_BY VARCHAR2(50)';
      END IF;

      /* (repeat this exact pattern for all remaining ALTER TABLE adds) */

      /* ============================================================
         DATA INSERT (SAFE)
         ============================================================ */
      MERGE INTO WEB_MENU_MANAGEMENT t
           USING (SELECT '77.01' menu_no FROM DUAL) s
              ON (t.menu_no = s.menu_no)
      WHEN NOT MATCHED
      THEN
         INSERT     (menu_no,
                     menu_edesc,
                     module_code,
                     full_path,
                     virtual_path,
                     group_sku_flag,
                     company_code,
                     created_by,
                     created_date)
             VALUES ('77.01',
                     'Quotation Approval',
                     '77',
                     '/QuotationManagement/Home/Index#!QM/QuotationApproval',
                     '/QuotationManagement/Home/Index#!QM/QuotationApproval',
                     'I',
                     '01',
                     '10',
                     SYSDATE);

      /* ============================================================
       SAFE COLUMN ADDITIONS
       ============================================================ */

      -- SA_QUOTATION_SETUP.APPROVED_BY
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'SA_QUOTATION_SETUP'
             AND COLUMN_NAME = 'APPROVED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE SA_QUOTATION_SETUP ADD APPROVED_BY VARCHAR2(100)';
      END IF;

      -- SA_QUOTATION_SETUP.LOCAL_FLAG
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'SA_QUOTATION_SETUP' AND COLUMN_NAME = 'LOCAL_FLAG';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE SA_QUOTATION_SETUP ADD LOCAL_FLAG CHAR(1) DEFAULT ''Y'' NOT NULL';
      END IF;

      -- SA_QUOTATION_SETUP.FORM_CODE
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'SA_QUOTATION_SETUP' AND COLUMN_NAME = 'FORM_CODE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE SA_QUOTATION_SETUP ADD FORM_CODE VARCHAR2(10)';
      END IF;

      -- SA_QUOTATION_SETUP.VAT_INCLUDE_FLAG
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'SA_QUOTATION_SETUP'
             AND COLUMN_NAME = 'VAT_INCLUDE_FLAG';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE SA_QUOTATION_SETUP ADD VAT_INCLUDE_FLAG CHAR(1) DEFAULT ''Y'' NOT NULL';
      END IF;

      -- SA_QUOTATION_ITEMS.QUOTATION_NO
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'SA_QUOTATION_ITEMS'
             AND COLUMN_NAME = 'QUOTATION_NO';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE SA_QUOTATION_ITEMS ADD QUOTATION_NO NUMBER';
      END IF;

      -- SA_QUOTATION_ITEMS.FORM_CODE
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'SA_QUOTATION_ITEMS' AND COLUMN_NAME = 'FORM_CODE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE SA_QUOTATION_ITEMS ADD FORM_CODE VARCHAR2(10)';
      END IF;

      -- QUOTATION_DETAILS.checked_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'QUOTATION_DETAILS' AND COLUMN_NAME = 'CHECKED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD CHECKED_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAILS.recommended_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAILS'
             AND COLUMN_NAME = 'RECOMMENDED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD RECOMMENDED_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAILS.posted_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'QUOTATION_DETAILS' AND COLUMN_NAME = 'POSTED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD POSTED_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAILS.checked_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAILS'
             AND COLUMN_NAME = 'CHECKED_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD CHECKED_DATE DATE';
      END IF;

      -- QUOTATION_DETAILS.recommended_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAILS'
             AND COLUMN_NAME = 'RECOMMENDED_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD RECOMMENDED_DATE DATE';
      END IF;

      -- QUOTATION_DETAILS.posted_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'QUOTATION_DETAILS' AND COLUMN_NAME = 'POSTED_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD POSTED_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.checked_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'CHECKED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD CHECKED_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.checked_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'CHECKED_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD CHECKED_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended1_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED1_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED1_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended1_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED1_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED1_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended2_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED2_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED2_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended2_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED2_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED2_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended3_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED3_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED3_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended3_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED3_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED3_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended4_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED4_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED4_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.recommended4_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'RECOMMENDED4_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD RECOMMENDED4_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.verify_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'VERIFY_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD VERIFY_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.verify_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'VERIFY_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD VERIFY_DATE DATE';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.approved_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'APPROVED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD APPROVED_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAIL_ITEMWISE.approved_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'QUOTATION_DETAIL_ITEMWISE'
             AND COLUMN_NAME = 'APPROVED_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAIL_ITEMWISE ADD APPROVED_DATE DATE';
      END IF;

      -- QUOTATION_DETAILS.verify_by
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'QUOTATION_DETAILS' AND COLUMN_NAME = 'VERIFY_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD VERIFY_BY VARCHAR2(50)';
      END IF;

      -- QUOTATION_DETAILS.verify_date
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'QUOTATION_DETAILS' AND COLUMN_NAME = 'VERIFY_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE QUOTATION_DETAILS ADD VERIFY_DATE DATE';
      END IF;

      -- WEB_MENU_CONTROL.APPROVE_FLAG
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'WEB_MENU_CONTROL' AND COLUMN_NAME = 'APPROVE_FLAG';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE WEB_MENU_CONTROL ADD APPROVE_FLAG CHAR(1)';
      END IF;

      -- WEB_MENU_CONTROL.RECOMMEND_FLAG
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'WEB_MENU_CONTROL'
             AND COLUMN_NAME = 'RECOMMEND_FLAG';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE WEB_MENU_CONTROL ADD RECOMMEND_FLAG CHAR(1)';
      END IF;

      -- WEB_MENU_CONTROL.VERIFY_FLAG
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE TABLE_NAME = 'WEB_MENU_CONTROL' AND COLUMN_NAME = 'VERIFY_FLAG';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE WEB_MENU_CONTROL ADD VERIFY_FLAG CHAR(1)';
      END IF;

      -- MASTER_TRANSACTION.RECOMMENDED_BY
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'MASTER_TRANSACTION'
             AND COLUMN_NAME = 'RECOMMENDED_BY';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE MASTER_TRANSACTION ADD RECOMMENDED_BY VARCHAR2(30)';
      END IF;

      -- MASTER_TRANSACTION.RECOMMENDED_DATE
      SELECT COUNT (*)
        INTO v_cnt
        FROM USER_TAB_COLS
       WHERE     TABLE_NAME = 'MASTER_TRANSACTION'
             AND COLUMN_NAME = 'RECOMMENDED_DATE';

      IF v_cnt = 0
      THEN
         EXECUTE IMMEDIATE
            'ALTER TABLE MASTER_TRANSACTION ADD RECOMMENDED_DATE DATE';
      END IF;


      /* ============================================================
         MARK SCRIPT EXECUTED
         ============================================================ */
      EXECUTE IMMEDIATE
         '
    INSERT INTO DB_SCRIPT_LOG (SCRIPT_NAME, EXECUTED_ON)
    VALUES (:1, SYSDATE)
  '
         USING 'QUOTATION_MODULE_V1';

      COMMIT;
   END IF;
END;
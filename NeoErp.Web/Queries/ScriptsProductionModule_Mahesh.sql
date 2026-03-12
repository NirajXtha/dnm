
/*
-----------------------------------------------------------------------------------
 Database Scripts: PRODUCTOION Module And Related Changes
 - Includes Tables, Views, newly added Columns, and Functions.

 Additional Steps:
 - Added the steps to display the menu after maintaining the record in the DB.
-----------------------------------------------------------------------------------
*/


DECLARE
   v_count NUMBER;
BEGIN
   -- Check if table exists in current schema
   SELECT COUNT(*)
   INTO v_count
   FROM user_tables
   WHERE table_name = 'MP_PRODUCTION_PREFERENCE';

   -- If not exists, then create
   IF v_count = 0 THEN
      EXECUTE IMMEDIATE '
         CREATE TABLE MP_PRODUCTION_PREFERENCE
         (
           REQUISITION_FORM_CODE   VARCHAR2(10 BYTE),
           INDENT_FORM_CODE        VARCHAR2(10 BYTE),
           BRANCH_CODE             VARCHAR2(30 BYTE)     NOT NULL,
           CREATED_BY              VARCHAR2(20 BYTE),
           CREATED_DATE            DATE,
           MODIFY_DATE             DATE,
           MODIFY_BY               VARCHAR2(30 BYTE),
           SYN_ROWID               VARCHAR2(18 BYTE),
           MODULEORSCREEN          VARCHAR2(80 BYTE),
           COMPANY_CODE            VARCHAR2(30 BYTE)     DEFAULT ''DEFAULT_VALUE'' NOT NULL,
           REQ_FROM_LOCATION_CODE  VARCHAR2(50 BYTE),
           REQ_TO_LOCATION_CODE    VARCHAR2(50 BYTE),
           IND_FROM_LOCATION_CODE  VARCHAR2(50 BYTE),
           IND_TO_LOCATION_CODE    VARCHAR2(50 BYTE)
         )
      ';
   END IF;
END;


--select * from MP_PRODUCTION_PREFERENCE;


--SELECT get_next_tracking_no FROM dual

CREATE OR REPLACE FUNCTION get_next_tracking_no
RETURN VARCHAR2 IS
    v_prefix       VARCHAR2(8);
    v_max_suffix   VARCHAR2(8);
    v_next_number  NUMBER;
    v_next_suffix  VARCHAR2(8);
    v_tracking_no  VARCHAR2(16);
BEGIN
    -- Get current date as YYYYMMDD
    v_prefix := TO_CHAR(SYSDATE, 'YYYYMMDD');

    -- Get max suffix for today's date
    SELECT NVL(MAX(SUBSTR(tracking_no, 9)), '00000000')
    INTO v_max_suffix
    FROM IP_PRODUCTION_ISSUE
    WHERE SUBSTR(tracking_no, 1, 8) = v_prefix;

    -- Calculate next number
    v_next_number := TO_NUMBER(v_max_suffix) + 1;

    -- Format suffix as 8-digit string with leading zeros
    v_next_suffix := LPAD(v_next_number, 8, '0');

    -- Combine prefix and suffix
    v_tracking_no := v_prefix || v_next_suffix;

    RETURN v_tracking_no;
END;




--SELECT get_next_tracking_no FROM dual


CREATE OR REPLACE FUNCTION get_next_tracking_no_mrr
RETURN VARCHAR2 IS
    v_prefix       VARCHAR2(8);
    v_max_suffix   VARCHAR2(8);
    v_next_number  NUMBER;
    v_next_suffix  VARCHAR2(8);
    v_tracking_no  VARCHAR2(16);
BEGIN
    -- Get current date as YYYYMMDD
    v_prefix := TO_CHAR(SYSDATE, 'YYYYMMDD');

    -- Get max suffix for today's date
    SELECT NVL(MAX(SUBSTR(tracking_no, 9)), '00000000')
    INTO v_max_suffix
    FROM IP_PRODUCTION_MRR
    WHERE SUBSTR(tracking_no, 1, 8) = v_prefix;

    -- Calculate next number
    v_next_number := TO_NUMBER(v_max_suffix) + 1;

    -- Format suffix as 8-digit string with leading zeros
    v_next_suffix := LPAD(v_next_number, 8, '0');

    -- Combine prefix and suffix
    v_tracking_no := v_prefix || v_next_suffix;

    RETURN v_tracking_no;
END;



--# insert fiscal year entry to get all that date option : this week, this month, last week , last month etc Current Fiscal year Dates.
-- also needed to change in web config file ; having Key     <add key="FiscalYear" value="2082/83" />  WEBCONFIG FILE
INSERT INTO HR_FISCAL_YEAR_CODE (
                    FISCAL_YEAR_CODE, START_DATE, END_DATE, EREMARKS, NREMARKS, 
                    COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                    DELETED_FLAG, SYN_ROWID, MODIFY_DATE, MODIFY_BY
                )
                VALUES (
                    '2082/83', TO_DATE('7/16/2025', 'MM/DD/YYYY'), TO_DATE('07/15/2026', 'MM/DD/YYYY'),
                    NULL, NULL,
                    '01', '01.01', 'ADMIN', TO_DATE('9/03/2025 12:31:36 PM', 'MM/DD/YYYY HH:MI:SS AM'),
                    'N', NULL, NULL, NULL
                ); 
                

--##    V_MP_PLAN_WISE_PORD_QTY_NEW     -- VIEW           
--##             

CREATE OR REPLACE FORCE VIEW V_MP_PLAN_WISE_PORD_QTY_NEW
(
   PLAN_CODE,
   LOCATION_CODE,
   ITEM_CODE,
   ITEM_EDESC,
   COMPANY_CODE,
   QUANTITY,
   PROD_QTY,
   PROD_REC,
   RECEIVED_QTY
)
AS
   SELECT PLAN_CODE,
          LOCATION_CODE,
          ITEM_CODE,
          ITEM_EDESC,
          COMPANY_CODE,
          QUANTITY,
          PROD_QTY,
          PROD_REC,
          (PROD_QTY - PROD_REC) RECEIVED_QTY
     FROM (SELECT DISTINCT
                  MP.PLAN_CODE,
                  LOCATION_CODE,
                  INDEX_ITEM_CODE ITEM_CODE,
                  ITEM_EDESC,
                  MP.COMPANY_CODE,
                  CASE
                     WHEN MPP.INDEX_ITEM_CODE = OP.ITEM_CODE
                     THEN
                        OP.PLAN_QUANTITY
                     WHEN OP.ITEM_CODE = '0'
                     THEN
                        (SELECT REQUIRED_QUANTITY
                           FROM MP_VARIANCE_INFO
                          WHERE     PLAN_CODE = MP.PLAN_CODE
                                AND (   FINISHED_ITEM_CODE =
                                           MPP.INDEX_ITEM_CODE
                                     OR RAW_ITEM_CODE = MPP.INDEX_ITEM_CODE)
                                AND COMPANY_CODE = MP.COMPANY_CODE
                                AND ROWNUM = 1)
                     ELSE
                        (SELECT REQUIRED_QUANTITY
                           FROM MP_VARIANCE_INFO
                          WHERE     PLAN_CODE = MP.PLAN_CODE
                                AND RAW_ITEM_CODE = MPP.INDEX_ITEM_CODE
                                AND COMPANY_CODE = MP.COMPANY_CODE
                                AND ROWNUM = 1)
                  END
                     QUANTITY,
                  NVL (
                     (SELECT SUM (PRODUCTION_QTY)
                        FROM IP_PRODUCTION_ISSUE
                       WHERE     PLAN_NO = OP.PLAN_NO
                             AND COMPANY_CODE = OP.COMPANY_CODE
                             AND DELETED_FLAG = 'N'
                             AND SERIAL_NO = 1
                             AND to_location_code = MPP.location_code),
                     0)
                     PROD_QTY,
                  NVL (
                     (SELECT SUM (PRODUCTION_QTY)
                        FROM IP_PRODUCTION_MRR
                       WHERE     PLAN_NO = OP.PLAN_NO
                             AND COMPANY_CODE = OP.COMPANY_CODE
                             AND DELETED_FLAG = 'N'
                             AND SERIAL_NO = 1),
                     0)
                     PROD_REC
             FROM MP_VARIANCE_INFO MP,
                  MP_PROCESS_SETUP MPP,
                  IP_ITEM_MASTER_SETUP IT,
                  MP_ORDER_PLAN_PROCESS OP
            WHERE     1 = 1
                  AND MP.PROCESS_CODE = MPP.PROCESS_CODE
                  AND MP.COMPANY_CODE = MPP.COMPANY_CODE
                  AND MPP.INDEX_ITEM_CODE = IT.ITEM_CODE
                  AND MPP.COMPANY_CODE = IT.COMPANY_CODE
                  AND MP.PLAN_CODE = OP.PLAN_NO
                  AND MP.COMPANY_CODE = OP.COMPANY_CODE
                  AND MP.DELETED_FLAG = 'N');
                


--Default_Value update
update Form_Detail_Setup set DEFA_VALUE=null where Table_Name='IP_PRODUCTION_ISSUE' and Column_Name='TO_LOCATION_CODE'; 

update Form_Detail_Setup set DEFA_VALUE=null where Table_Name='IP_PRODUCTION_MRR' and Column_Name='TO_LOCATION_CODE';

-- ## Production Plan and Routine Process. End





------##-------------------------------------------------------------##------              

--## Process Setup Menu
DECLARE
   v_exists NUMBER;
BEGIN
   -- Check if FULL_PATH already exists
   SELECT COUNT(*)
     INTO v_exists
     FROM WEB_MENU_MANAGEMENT
    WHERE FULL_PATH = '/DocumentTemplate/Home/Index#!DT/ProcessSetup';

   IF v_exists = 0 THEN
     

      -- Insert with calculated MENU_NO
      INSERT INTO WEB_MENU_MANAGEMENT (
         MENU_NO,
         MENU_EDESC,
         MENU_OBJECT_NAME,
         MODULE_CODE,
         FULL_PATH,
         VIRTUAL_PATH,
         ICON_PATH,
         GROUP_SKU_FLAG,
         PRE_MENU_NO,
         COMPANY_CODE,
         CREATED_BY,
         CREATED_DATE,
         ORDERBY,
         MODULE_ABBR,
         COLOR
      )
      VALUES (
      
         (SELECT SUBSTR (MAX (MENU_NO), 1, INSTR (MAX (MENU_NO), '.'))
            || LPAD (
                  MAX (TO_NUMBER (SUBSTR (MENU_NO, INSTR (MENU_NO, '.') + 1))) + 1,
                  2,
                  '0')
     FROM WEB_MENU_MANAGEMENT
    WHERE FULL_PATH LIKE '%/DocumentTemplate/Home/Index%'),
    
         'Process',
         'Process',
         '05',
         '/DocumentTemplate/Home/Index#!DT/ProcessSetup',
         '/DocumentTemplate/Home/Index#!DT/ProcessSetup',
         'fa fa-sitemap',
         'I',
         '00',
         '01',
         '01',
         TO_DATE('02/19/2021 18:17:36', 'MM/DD/YYYY HH24:MI:SS'),
         1,
         'RS',
         '#808080'
      );

      COMMIT;
   ELSE
      DBMS_OUTPUT.PUT_LINE('FULL_PATH already exists, skipping insert.');
   END IF;
END;


-- Select * from WEB_MENU_MANAGEMENT  WHERE  MENU_NO='05.32'
-- Select * from WEB_MENU_MANAGEMENT  WHERE  FULL_PATH like '%/DocumentTemplate/Home/Index%';


--## Process Setup BOM  Menu Under Master Setup
DECLARE
   v_exists_bom NUMBER;
BEGIN
   -- Check if FULL_PATH already exists
   SELECT COUNT(*)
     INTO v_exists_bom
     FROM WEB_MENU_MANAGEMENT
    WHERE FULL_PATH = '/DocumentTemplate/Home/Index#!DT/processSetupBom';

   IF v_exists_bom = 0 THEN
     
        
   
   INSERT INTO WEB_MENU_MANAGEMENT (
    MENU_NO,
    MENU_EDESC,
    MENU_OBJECT_NAME,
    MODULE_CODE,
    FULL_PATH,
    VIRTUAL_PATH,
    ICON_PATH,
    GROUP_SKU_FLAG,
    PRE_MENU_NO,
    COMPANY_CODE,
    CREATED_BY,
    CREATED_DATE,
    ORDERBY,
    MODULE_ABBR,
    COLOR
)
VALUES (
             (SELECT SUBSTR (MAX (MENU_NO), 1, INSTR (MAX (MENU_NO), '.'))
            || LPAD (
                  MAX (TO_NUMBER (SUBSTR (MENU_NO, INSTR (MENU_NO, '.') + 1))) + 1,
                  2,
                  '0')
     FROM WEB_MENU_MANAGEMENT
    WHERE FULL_PATH LIKE '%/DocumentTemplate/Home/Index%'),
    
    
    
    
    'Process Setup BOM',
    'Process Setup BOM',
    '05',
    '/DocumentTemplate/Home/Index#!DT/processSetupBom',
    '/DocumentTemplate/Home/Index#!DT/processSetupBom',
    'fa fa-sitemap',
    'I',
    '00',
    '01',
    '01',
    TO_DATE('02/19/2021 18:17:36', 'MM/DD/YYYY HH24:MI:SS'),
    1,
    'RS',
    '#808080'
);

   

      COMMIT;
      
   ELSE
      DBMS_OUTPUT.PUT_LINE('FULL_PATH already exists, skipping insert.');
   END IF;
END;

 -- Select * from WEB_MENU_MANAGEMENT  WHERE  MENU_NO = '05.33';
 -- Select * from WEB_MENU_MANAGEMENT  WHERE  FULL_PATH like '%/processSetupBom%';
 
 
 
 
 --## RESOURCE SETUP
 
DECLARE
   v_exists_r NUMBER;
BEGIN
   -- Check if FULL_PATH already exists
   SELECT COUNT(*)
     INTO v_exists_r
     FROM WEB_MENU_MANAGEMENT
    WHERE FULL_PATH =  '/DocumentTemplate/Home/Index#!DT/ResourceSetup';

   IF v_exists_r = 0 THEN
     
        
  /* Formatted on 9/4/2025 11:01:22 AM (QP5 v5.215.12089.38647) */
INSERT INTO IPPLMT.WEB_MENU_MANAGEMENT (MENU_NO,
                                        MENU_EDESC,
                                        MENU_OBJECT_NAME,
                                        MODULE_CODE,
                                        FULL_PATH,
                                        VIRTUAL_PATH,
                                        ICON_PATH,
                                        GROUP_SKU_FLAG,
                                        PRE_MENU_NO,
                                        COMPANY_CODE,
                                        CREATED_BY,
                                        CREATED_DATE,
                                        ORDERBY,
                                        MODULE_ABBR,
                                        COLOR)
     VALUES (
     
     
                 (SELECT SUBSTR (MAX (MENU_NO), 1, INSTR (MAX (MENU_NO), '.'))
            || LPAD (
                  MAX (TO_NUMBER (SUBSTR (MENU_NO, INSTR (MENU_NO, '.') + 1))) + 1,
                  2,
                  '0')
     FROM WEB_MENU_MANAGEMENT
    WHERE FULL_PATH LIKE '%/DocumentTemplate/Home/Index%'),
     
     
     
             'Resource',
             'Resource',
             '05',
             '/DocumentTemplate/Home/Index#!DT/ResourceSetup',
             '/DocumentTemplate/Home/Index#!DT/ResourceSetup',
             'fa fa-sitemap',
             'I',
             '00',
             '01',
             '01',
             TO_DATE ('02/19/2021 18:17:36', 'MM/DD/YYYY HH24:MI:SS'),
             1,
             'RS',
             '#808080   ');
   
   

      COMMIT;
      
   ELSE
      DBMS_OUTPUT.PUT_LINE('FULL_PATH already exists, skipping insert.');
   END IF;
END;




/*
-----------------------------------------------------------------------------------
 Checks:
 - Run to verify existing menu entries:
   SELECT * 
   FROM WEB_MENU_MANAGEMENT  
   WHERE MENU_NO = '05.34';

   SELECT * 
   FROM WEB_MENU_MANAGEMENT  
   WHERE FULL_PATH LIKE '%/Production%';

 Purpose:
 - This is for Process Setup, Process BOM Setup, and Resource Setup Menu.

 Note:
 - Based on the related DB UserId, we need to manage the menu by maintaining
   the entries in 'WEB_MENU_CONTROL'.
 - Currently, this is a manual process.
 
 WEB_MENU_MANAGEMENT table is used to maintain the record to make manu for :  Process Setup, Process BOM Setup, and Resource Setup. 
-----------------------------------------------------------------------------------
*/
  
-- 
--select * from sc_application_users where company_code='01';
--select * from WEB_MENU_MANAGEMENT where MENU_EDESC in ('Process','Resource','Process Setup BOM');
-- 
-- 
-- Insert into WEB_MENU_CONTROL
--   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
--    CREATED_DATE)
-- Values
--   (60747, '05.10', 'Y', '01', '35.01', 
--    TO_DATE('08/26/2025 12:45:22', 'MM/DD/YYYY HH24:MI:SS'));
--    
-- Commit;
--       
--    
--    
--    
--Insert into WEB_MENU_CONTROL
--   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
--    CREATED_DATE)
-- Values
--   (60747, '05.32', 'Y', '01', '35.01', 
--    TO_DATE('08/26/2025 12:45:22', 'MM/DD/YYYY HH24:MI:SS'));
--    
--Commit;
--    
--    
-- 
--Insert into WEB_MENU_CONTROL
--   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
--    CREATED_DATE)
-- Values
--   (60747, '05.33', 'Y', '01', '35.01', 
--    TO_DATE('08/26/2025 12:45:22', 'MM/DD/YYYY HH24:MI:SS'));
--    
--Commit;


--## Regarding Resource Setup
-------------------------------------------------------------------------------------


--select * from MP_RESOURCE_SETUP
-- Adding Human Resource and Human Resource type as Resource Category
 

ALTER TABLE MP_RESOURCE_SETUP
ADD (
  PURCHASE_DATE     DATE,
  EFFECTIVE_DATE    DATE,
  OUTPUT_CAPACITY   NUMBER(10,2),
  UNIT              VARCHAR2(20 BYTE),
  IS_SERIALS        CHAR(1 BYTE) DEFAULT 'N' CHECK (IS_SERIALS IN ('Y', 'N'))
);




/* Formatted on 9/4/2025 12:01:59 PM (QP5 v5.215.12089.38647) */
INSERT INTO MP_RESOURCE_SETUP (RESOURCE_CODE,
                               RESOURCE_EDESC,
                               RESOURCE_NDESC,
                               GROUP_SKU_FLAG,
                               PRE_RESOURCE_CODE,
                               REMARKS,
                               COMPANY_CODE,
                               CREATED_BY,
                               CREATED_DATE,
                               DELETED_FLAG,
                               IS_SERIALS)
     VALUES ('04',
             'Human Resource',
             'Menpower Type',
             'G',
             '00',
             'Menpower Type ',
             '01',
             'ADMIN',
             TO_DATE ('07/31/2025 14:18:01', 'MM/DD/YYYY HH24:MI:SS'),
             'N',
             'N');
 COMMIT;
    
    
/* Formatted on 9/4/2025 12:03:55 PM (QP5 v5.215.12089.38647) */
INSERT INTO MP_RESOURCE_SETUP (RESOURCE_CODE,
                               RESOURCE_EDESC,
                               RESOURCE_NDESC,
                               GROUP_SKU_FLAG,
                               PRE_RESOURCE_CODE,
                               REMARKS,
                               COMPANY_CODE,
                               CREATED_BY,
                               CREATED_DATE,
                               DELETED_FLAG,
                               IS_SERIALS)
     VALUES ('05',
             'Comsumable/Power Based',
             'consumable',
             'G',
             '00',
             'consumable',
             '01',
             'ADMIN',
             TO_DATE ('08/03/2025 10:04:51', 'MM/DD/YYYY HH24:MI:SS'),
             'N',
             'N');
COMMIT;

-------------------------------------------------------------------


DECLARE
    v_countmrt NUMBER;
BEGIN
    -- Check if the table exists
    SELECT COUNT(*)
    INTO v_countmrt
    FROM USER_TABLES
    WHERE TABLE_NAME = 'MP_RESOURCE_DETAIL';

    IF v_countmrt = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE TABLE MP_RESOURCE_DETAIL
            (
              RESOURCE_DETAIL_ID    VARCHAR2(36 BYTE)       NOT NULL,
              RESOURCE_CODE         VARCHAR2(30 BYTE)       NOT NULL,
              COMPANY_CODE          VARCHAR2(30 BYTE)       NOT NULL,
              RESOURCE_UNIQUE_NAME  VARCHAR2(100 BYTE)      NOT NULL,
              SERIAL_NO             VARCHAR2(50 BYTE)       NOT NULL,
              PURCHASE_DATE         DATE                    NOT NULL,
              EFFECTIVE_DATE        DATE,
              OUTPUT_CAPACITY       NUMBER(10,2),
              UNIT                  VARCHAR2(20 BYTE),
              CREATED_BY            VARCHAR2(30 BYTE)       NOT NULL,
              CREATED_DATE          DATE                    NOT NULL,
              MODIFY_BY             VARCHAR2(30 BYTE),
              MODIFY_DATE           DATE,
              DELETED_FLAG          CHAR(1 BYTE)            DEFAULT ''N''
            )';
    END IF;
END;


-- select * from MP_ROUTINE_RESOURCE_SETUP
-- changes of MP_ROUTINE_RESOURCE_SETUP table

ALTER TABLE MP_ROUTINE_RESOURCE_SETUP 
  ADD (
    CATEGORY_TYPE        VARCHAR2(2 BYTE),
    STANDARD_OUTPUT_QTY  NUMBER(14,4),
    STANDARD_INPUT_QTY   NUMBER(14,4)
  );

ALTER TABLE MP_ROUTINE_RESOURCE_SETUP 
MODIFY (MU_CODE VARCHAR2(30 BYTE));


--select * from MP_RESOURCE_DETAIL_ENTRY;
-- changes of MP_RESOURCE_DETAIL_ENTRY table

ALTER TABLE MP_RESOURCE_DETAIL_ENTRY
  ADD (
    EXPECTED_QTY   NUMBER(14,4),
    ACTUAL_QTY     NUMBER(14,4),
    CATEGORY_TYPE  VARCHAR2(2 BYTE),
    SERIAL_NO      VARCHAR2(70 BYTE)
  );
  
 
/*
-----------------------------------------------------------------------------------
 Do not run these steps directly; uncomment as needed.
 Read carefully and run accordingly.  

 Menu for Production Planning:
 - Managing the menu in the application is a manual process with data insertion 
   into the tables.
 - The below scripts help to show 'Production Planning' under the Production 
   Management menu.
 - First, check if entry with code 501 exists in the target DB.
   * If it exists, maintain the entry accordingly (create below entry with a 
     unique form code).
   * Otherwise, just run the below query and commit.
-----------------------------------------------------------------------------------
*/

--INSERT INTO IPPLMT.FORM_SETUP (FORM_CODE,
--                               FORM_EDESC,
--                               FORM_NDESC,
--                               MASTER_FORM_CODE,
--                               PRE_FORM_CODE,
--                               MODULE_CODE,
--                               GROUP_SKU_FLAG,
--                               CUSTOM_SUFFIX_TEXT,
--                               BODY_LENGTH,
--                               START_NO,
--                               LAST_NO,
--                               START_DATE,
--                               LAST_DATE,
--                               COMPANY_CODE,
--                               CREATED_BY,
--                               CREATED_DATE,
--                               DELETED_FLAG,
--                               FORM_ACTION_FLAG,
--                               TOTAL_ROUND_INDEX,
--                               DELTA_FLAG,
--                               SYN_ROWID,
--                               FREEZE_BACK_DAYS,
--                               DECIMAL_PLACE,
--                               FORM_TYPE,
--                               MODIFY_DATE,
--                               PRICE_CONTROL_FLAG,
--                               ACCESS_BDFSM_FLAG,
--                               INFO_FLAG,
--                               VNO_AS_DOC_ID_CONTROL,
--                               DEFAULT_SHIPMENT,
--                               RECEIPT_FLAG,
--                               NON_RETURN_FLAG,
--                               DISPLAY_RATE,
--                               EMAIL_FLAG,
--                               DEACTIVATE_TAXABLE_RATE_FLAG,
--                               AUTO_CONV_FLAG,
--                               PROD_IO_EQUAL_FLAG,
--                               QR_FLAG,
--                               CALL_API
--                               )
--     VALUES ('501',
--             'Production Planning',
--             'Production Planning',
--             '17',
--             '00',
--             '03',
--             'I',
--             '/73-74',
--             6,
--             1,
--             999999,
--             TO_DATE ('07/16/2024 00:00:00', 'MM/DD/YYYY HH24:MI:SS'),
--             TO_DATE ('07/15/2025 00:00:00', 'MM/DD/YYYY HH24:MI:SS'),
--             '01',
--             'ADMIN',
--             TO_DATE ('01/26/2010 00:00:00', 'MM/DD/YYYY HH24:MI:SS'),
--             'N',
--             '100',
--             2,
--             'N',
--             'AAAHdVAAOAAAApLAAB',
--             80,
--             2,
--             'OT',
--             TO_DATE ('05/28/2016 12:47:32', 'MM/DD/YYYY HH24:MI:SS'),
--             'N',
--             'N',
--             'Y',
--             'N',
--             'None',
--             'N',
--             'N',
--             'N',
--             'N',
--             'N',
--             'Y',
--             'N',
--             'N',
--             'N'
--             ); 


/*
-----------------------------------------------------------------------------------
 Next step:
 - Now you need to maintain the record in table 'SC_FORM_CONTROL'.
 - The script has been created based on the targeted DB UserId.
   * Note: UserId may differ depending on the database.
 - Use the following query to check the UserId first:

   SELECT * 
   FROM sc_application_users 
   WHERE company_code = '01';
-----------------------------------------------------------------------------------
*/



/* Formatted on 9/7/2025 12:32:05 PM (QP5 v5.215.12089.38647) */
--INSERT INTO SC_FORM_CONTROL (USER_NO,
--                             FORM_CODE,
--                             CREATE_FLAG,
--                             READ_FLAG,
--                             UPDATE_FLAG,
--                             DELETE_FLAG,
--                             POST_FLAG,
--                             UNPOST_FLAG,
--                             CHECK_FLAG,
--                             VERIFY_FLAG,
--                             MORE_FLAG,
--                             COMPANY_CODE,
--                             CREATED_BY,
--                             CREATED_DATE,
--                             DELETED_FLAG,
--                             BRANCH_CODE)
--     VALUES (60747,
--             '501',
--             'Y',
--             'Y',
--             'Y',
--             'Y',
--             'Y',
--             'Y',
--             'Y',
--             'Y',
--             '0',
--             '01',
--             'ADMIN',
--             TO_DATE ('08/17/2025 16:30:23', 'MM/DD/YYYY HH24:MI:SS'),
--             'N',
--             '01.01');


/*
-----------------------------------------------------------------------------------
 Result:
 - Now you can see the 'Production Planning' menu 
   under the 'Production Management' section.
-----------------------------------------------------------------------------------
*/ 




--- Updated
--- Remove unwanted entry field for production(routine/bom process); for table IP_PRODUCTION_ISSUE
--- select * from Form_Detail_Setup where Form_Code=128;
  
  
  UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_ISSUE'
  AND Column_Name = 'CALC_TOTAL_PRICE'
  AND Company_Code = '01';


  UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_ISSUE'
  AND Column_Name = 'COMPLETED_QUANTITY'
  AND Company_Code = '01';


  UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_ISSUE'
  AND Column_Name = 'CALC_UNIT_PRICE'
  AND Company_Code = '01';


  UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_ISSUE'
  AND Column_Name = 'TOTAL_PRICE'
  AND Company_Code = '01';



  UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_ISSUE'
  AND Column_Name = 'UNIT_PRICE'
  AND Company_Code = '01';



UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_ISSUE'
  AND Column_Name = 'UNIT_PRICE'
  AND Company_Code = '01';


----- Updated
----- Remove unwanted entry field for production(routine/bom process); for table IP_PRODUCTION_MRR
----- select * from Form_Detail_Setup where Form_Code=129;
 
 UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_MRR'
  AND Column_Name = 'COMPLETED_QUANTITY'
  AND Company_Code = '01';


 UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_MRR'
  AND Column_Name = 'CALC_TOTAL_PRICE'
  AND Company_Code = '01';
  
  
  
 UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_MRR'
  AND Column_Name = 'CALC_UNIT_PRICE'
  AND Company_Code = '01';



 UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_MRR'
  AND Column_Name = 'TOTAL_PRICE'
  AND Company_Code = '01';


 UPDATE Form_Detail_Setup
SET DISPLAY_FLAG = 'N'
WHERE Table_Name = 'IP_PRODUCTION_MRR'
  AND Column_Name = 'UNIT_PRICE'
  AND Company_Code = '01';



---PLANNING Reference in BOM/ROUTINE process
----------------------------------------------------------------------------------
-- STEP 1: Check existing records for Form_Code = 128
----------------------------------------------------------------------------------
--select * from Form_Detail_Setup where Form_Code = 128;

----------------------------------------------------------------------------------
-- STEP 2: Check if COLUMN_NAME = 'PLAN_NO' already exists under Form_Code = 128
----------------------------------------------------------------------------------
--select * 
--from Form_Detail_Setup 
--where Form_Code = 128 
--  and Column_Name = 'PLAN_NO';

----------------------------------------------------------------------------------
-- STEP 3: Get the next available SERIAL_NO under Form_Code = 128
----------------------------------------------------------------------------------
WITH distinct_serials AS (
    SELECT DISTINCT serial_no
    FROM Form_Detail_Setup
    WHERE Form_Code = 128
),
missing AS (
    SELECT LEVEL AS next_serial
    FROM dual
    CONNECT BY LEVEL <= (SELECT MAX(serial_no) + 1 FROM distinct_serials)
    MINUS
    SELECT serial_no FROM distinct_serials
)
SELECT MIN(next_serial) AS next_available_serial
FROM missing;
-- 👆 Use the returned value for SERIAL_NO in the insert below.

----------------------------------------------------------------------------------
-- STEP 4: Insert PLAN_NO if not already present
-- ⚠️ Replace the SERIAL_NO below with the value from STEP 3 query
----------------------------------------------------------------------------------
INSERT INTO Form_Detail_Setup (
    SERIAL_NO,
    TABLE_NAME,
    COLUMN_NAME,
    COLUMN_WIDTH,
    COLUMN_HEADER,
    TOP_POSITION,
    LEFT_POSITION,
    DISPLAY_FLAG,
    DEFA_VALUE,
    IS_DESC_FLAG,
    MASTER_CHILD_FLAG,
    FORM_CODE,
    COMPANY_CODE,
    CREATED_BY,
    CREATED_DATE,
    DELETED_FLAG,
    FILTER_VALUE,
    SYN_ROWID,
    MODIFY_DATE,
    MODIFY_BY,
    HELP_DESCRIPTION
)
SELECT
    23,   -- ✅ replace with next_available_serial from STEP 3
    'IP_PRODUCTION_ISSUE',
    'PLAN_NO',
    2400,
    'Plan No',
    880,
    6800,
    'Y',
    NULL,
    'Y',
    'M',
    128,
    '01',
    'ADMIN',
    TO_DATE('02-APR-25', 'DD-MON-RR'),
    'N',
    NULL,
    NULL,
    NULL,
    NULL,
    'HELP_DESCRIPTION'
FROM dual
WHERE NOT EXISTS (
    SELECT 1
    FROM Form_Detail_Setup
    WHERE Form_Code = 128
      AND Column_Name = 'PLAN_NO'
);

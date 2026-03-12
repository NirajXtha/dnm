--hide vehicle registration
UPDATE WEB_MENU_CONTROL
SET ACCESS_FLAG = 'N'
WHERE MENU_NO ='05.17';

--for Update Menu
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.01', 'Rate Schedule', 'Rate Schedule', '09', '/DocumentTemplate/Home/Index#!DT/RateSchedule',
  '/DocumentTemplate/Home/Index#!DT/RateSchedule', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/03/2025 12:13:56', 'MM/DD/YYYY HH24:MI:SS'), 1, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.02', 'Discount Schedule', 'Discount Schedule', '09', '/DocumentTemplate/Home/Index#!DT/DiscountSchedule',
  '/DocumentTemplate/Home/Index#!DT/DiscountSchedule', 'fa fa-edit', 'I', '00', '02',
  '01', TO_DATE('12/03/2025 12:13:56', 'MM/DD/YYYY HH24:MI:SS'), 1, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.03', 'Loading Slip Printer', 'Loading Slip Printer', '09', '/DocumentTemplate/Home/Index#!DT/LoadingSlipPrinter',
  '/DocumentTemplate/Home/Index#!DT/LoadingSlipPrinter', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 11:49:56', 'MM/DD/YYYY HH24:MI:SS'), 3, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.04', 'Loading Slip Generator', 'Loading Slip Generator', '09', '/DocumentTemplate/Home/Index#!DT/LoadingSlipGenerator',
  '/DocumentTemplate/Home/Index#!DT/LoadingSlipGenerator', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 11:49:56', 'MM/DD/YYYY HH24:MI:SS'), 4, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.05', 'Vehicle Registration', 'Vehicle Registration', '09', '/DocumentTemplate/Home/Index#!DT/VehicleRegistration',
  '/DocumentTemplate/Home/Index#!DT/VehicleRegistration', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 11:49:56', 'MM/DD/YYYY HH24:MI:SS'), 5, 'ACC', '#808080 ');
 
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.06', 'Bank Reconcilation', 'Bank Reconcilation', '09', '/DocumentTemplate/Home/Index#!DT/bankReconcilation',
  '/DocumentTemplate/Home/Index#!DT/bankReconcilation', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 11:49:56', 'MM/DD/YYYY HH24:MI:SS'), 6, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.07', 'Consumption Voucher Generate', 'Consumption Voucher Generate', '09', '/DocumentTemplate/Home/Index#!DT/ConsumptionVoucherGenerate',
  '/DocumentTemplate/Home/Index#!DT/ConsumptionVoucherGenerate', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 11:49:56', 'MM/DD/YYYY HH24:MI:SS'), 7, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.08', 'Bank Gurantee', 'Bank Gurantee', '09', '/DocumentTemplate/Home/Index#!DT/bankGurantee',
  '/DocumentTemplate/Home/Index#!DT/bankGurantee', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/03/2025 12:13:56', 'MM/DD/YYYY HH24:MI:SS'), 8, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.09', 'Post Date Cheque', 'Post Date Cheque', '09', '/DocumentTemplate/Home/Index#!DT/PostDateCheque',
  '/DocumentTemplate/Home/Index#!DT/PostDateCheque', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 11:49:56', 'MM/DD/YYYY HH24:MI:SS'), 9, 'ACC', '#808080 ');
Insert into WEB_MENU_MANAGEMENT
  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH,
  VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE,
  CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
 Values
  ('09.10', 'Gate Entry', 'Gate Entry', '09', '/DocumentTemplate/Home/Index#!DT/GateEntry',
  '/DocumentTemplate/Home/Index#!DT/GateEntry', 'fa fa-edit', 'I', '00', '01',
  '01', TO_DATE('12/04/2025 12:39:56', 'MM/DD/YYYY HH24:MI:SS'), 10, 'ACC', '#808080 ');

  --WEB_CONTROL
Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.01', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.02', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.03', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.04', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.05', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.06', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.07', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.08', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.09', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

Insert into WEB_MENU_CONTROL
   (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, 
    CREATED_DATE)
 Values
   (60747, '09.10', 'Y', '01', '60747', 
    TO_DATE('12/03/2025 2:27:56', 'MM/DD/YYYY HH24:MI:SS'));

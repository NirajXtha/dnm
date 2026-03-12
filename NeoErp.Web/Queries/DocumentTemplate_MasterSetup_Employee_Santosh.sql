
--Start: Santosh, 11Feb2026, PathSetup for Employee Setuop

INSERT into WEB_MENU_MANAGEMENT
    (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, 
    COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
Values 
    ('05.38', 'Employee', 'Employee', '05', 
    '/DocumentTemplate/Home/Index#!DT/EmployeeSetup', '/DocumentTemplate/Home/Index#!DT/EmployeeSetup', 'fa fa-recycle', 'I', '00', 
    '01', '60747',sysdate , 32, 'DT', '#808080   ');

INSERT INTO WEB_MENU_CONTROL
    (USER_NO, MENU_NO, ACCESS_FLAG, COMPANY_CODE, CREATED_BY, CREATED_DATE)
VALUES(60747, '05.38', 'Y', '01', '60747', sysdate);
--END: Santosh, 11Feb2026, PathSetup for Employee Setuop



--Start: Santosh, 12Feb2026, Process Setup is Removed from the database as Per Mahesh Dai's Instruction

DELETE FROM WEB_MENU_MANAGEMENT WHERE  MODULE_CODE = '05' AND FULL_PATH = '/DocumentTemplate/Home/Index#!DT/ProcessSetup' AND MENU_NO = '05.34';
DELETE FROM WEB_MENU_CONTROL WHERE MENU_NO = '05.34'

--END: Santosh, 12Feb2026, Process Setup is Removed from the database as Per Mahesh Dai's Instruction

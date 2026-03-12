# NeoERP Feature Documentation - Part 1

**Author:** Kushal Tiwari 
**Date:** October 2025
**Purpose:** Technical documentation 


## Table of Contents - Part 1
1. [Order Dispatch Management](#1-order-dispatch-management)
2. [Gate Entry](#2-gate-entry)
3. [Rate Schedule](#3-rate-schedule)
4. [Discount Schedule](#4-discount-schedule)
5. [Loading Slip Generate](#5-loading-slip-generate)
6. [Miscellaneous Sub Ledger Setup](#6-miscellaneous-sub-ledger-setup)

**See Part 2 for:** Attribute Setup, Dealer Setup, Account Nature Logic, Bank Mapping, Item Mapping, Dealer Mapping

## 1. Order Dispatch Management

### What It Does
Helps warehouse team plan and manage product deliveries to customers. Tracks which orders need to be sent, when, and monitors the delivery process.

### Why We Need It
- **Problem:** Manually tracking deliveries is chaotic and error-prone
- **Solution:** Centralized system to see all pending orders and plan deliveries
- **Benefit:** Reduces delays, improves customer satisfaction, helps prioritize work

### How It Works

**Step 1: Finding Orders Ready for Dispatch**
- Shows all approved sales orders waiting to be sent
- Calculates pending quantities (not yet delivered)
- Shows current stock availability
- Displays partially dispatched orders

**Why:** Gives complete picture of what needs to go out and what's available

**Step 2: Planning the Dispatch**
- Select orders and enter quantity to dispatch
- System validates quantity > 0
- Creates dispatch schedule with number, customer, items, quantities
- Marks as planned but not executed

**Why:** Separates planning from execution for review/approval

**Step 3: Tracking Status**
- Shows all planned dispatches with status
- Tracks warehouse acknowledgment
- Links to loading slip generation

**Why:** Provides visibility for management reporting

**Technical Info:**
- Controller: `OrderDispatchApiController.cs`
- Service: `OrderDispatchService.cs`
- Table: `ORDER_DISPATCH_SCHEDULE`

---

## 2. Gate Entry

### What It Does
Manages vehicles entering/exiting factory gate. Tracks vehicle details, driver info, weighbridge readings, and complete inward/outward process.

### Why We Need It
- **Problem:** Without gate management, we lose track of vehicles, materials, and timings
- **Solution:** Digital record of every vehicle entry/exit with weight measurements
- **Benefit:** Better security, accurate material tracking, proof of delivery/receipt

### How It Works

**Step 1: Vehicle Registration**
- Security records vehicle number, driver name, license, mobile
- System captures entry time automatically
- Records purpose (delivery, pickup, etc.)
- Can link to purchase/sales orders

**Why:** Creates audit trail of who came, when, and why

**Step 2: Weighbridge Process**
- First weighing (Gross Weight): Vehicle with load
- Tear Weight: Vehicle's empty weight
- Net Weight: Auto-calculated (Gross - Tear)
- Records both readings with timestamps

**Why:** Ensures accurate material quantity tracking, prevents disputes

**Step 3: Unloading/Loading**
- Records start and end time
- Captures location in factory
- Links to items and quantities
- Updates inventory when complete

**Why:** Identifies bottlenecks, improves efficiency

**Step 4: Gate Exit**
- Records exit time
- Calculates total time in factory
- Generates gate pass/loading slip
- Closes gate entry record

**Why:** Completes audit trail, provides turnaround time data

**Technical Info:**
- View: `GateEntry.cshtml`
- Table: `IP_VEHICLE_TRACK`
- Key Fields: `GROSS_WT`, `TEAR_WT`, `NET_WT`, `IN_TIME`, `OUT_TIME`

---

## 3. Rate Schedule

### What It Does
Manages different prices for same product based on who's buying (customer/dealer/supplier), when (effective date), and where (area). Price list management system.

### Why We Need It
- **Problem:** Different customers get different prices, manual management leads to errors
- **Solution:** Centralized system to set and track all pricing rules
- **Benefit:** Consistent pricing, easier discounts, automatic price application

### How It Works

**Step 1: Setting Up Rate Schedule**
- Select effective date (when prices become active)
- Choose currency and exchange rate
- Select area (if prices differ by location)
- Pick document type (sales invoice, quotation, etc.)

**Why:** Allows future-dated price changes and multi-currency pricing

**Step 2: Selecting Parties**
- Choose party type (Customer, Dealer, or Supplier)
- Select multiple parties using tree view
- Shows hierarchical structure

**Why:** Apply same prices to multiple parties at once

**Step 3: Setting Item Rates**
- Standard Rate: Base selling price
- MRP Rate: Maximum retail price
- Retail Price: Actual selling price
- Variation Rate: Percentage/amount adjustment

**Why:** Supports different pricing strategies

**Step 4: Saving and Applying**
- Deletes existing rates for same date/party/item
- Saves new rates
- Auto-applies when creating sales/purchase documents

**Why:** Prevents duplicates, ensures latest rates used

**Technical Info:**
- View: `RateSchedule.cshtml`
- Method: `SaveRateSchedule()` in `DocumentSetupRepo.cs`
- Table: `IP_ITEM_RATE_SCHEDULE_SETUP`

---

## 4. Discount Schedule

### What It Does
Manages discounts instead of prices. Sets up different discount percentages or amounts for different customers and items.

### Why We Need It
- **Problem:** Ad-hoc discounts lead to inconsistency and profit loss
- **Solution:** Pre-approved discount schedules that auto-apply
- **Benefit:** Controlled discounting, better margins, faster processing

### How It Works

**Step 1: Discount Configuration**
- Select effective date
- Choose currency and exchange rate
- Select charge code (discount type)
- Pick document type

**Why:** Allows seasonal, promotional, and customer-specific discounts

**Step 2: Party Selection**
- Select Customer, Dealer, or Supplier
- Multiple parties from tree view
- Group-level or individual-level selection

**Why:** Blanket discounts to all dealers or specific to VIP customers

**Step 3: Setting Discount Values**
- Choose: "As per %" or "As per Value"
- Discount Rate: Fixed amount off
- Discount Percent: Percentage off
- Item-specific overrides available

**Why:** Supports both flat and percentage discounts with item-level control

**Step 4: Application in Sales**
- Auto-checks for active discount schedule
- Matches customer, item, and date
- Applies highest applicable discount
- User can override if needed

**Why:** Reduces manual work, ensures approved discounts applied

**Technical Info:**
- View: `DiscountSchedule.cshtml`
- Method: `SaveDiscountSchedule()` in `DocumentSetupRepo.cs`
- Table: `IP_ITEM_DISCOUNT_SCHEDULE`

---

## 5. Loading Slip Generate

### What It Does
Creates loading slip document listing items being loaded onto vehicle for delivery. Links dispatch planning with actual vehicle loading.

### Why We Need It
- **Problem:** Without loading slips, drivers don't know what they're carrying
- **Solution:** Official document with all items, quantities, delivery details
- **Benefit:** Reduces errors, provides proof of dispatch, helps tracking

### How It Works

**Step 1: Selecting Dispatch Orders**
- Shows planned but not yet loaded dispatches
- Filter by date (Today, Yesterday, Last 7 days)
- Displays customer, items, quantities, destination

**Why:** Helps prioritize which orders to load first

**Step 2: Selecting Vehicle**
- Shows vehicles entered through gate
- Displays vehicle details, driver info, entry time
- Select appropriate vehicle

**Why:** Ensures only registered vehicles used, links gate entry with dispatch

**Step 3: Generating Loading Slip**
- Generates unique loading slip number
- Creates detail record for each item
- Updates vehicle track with loading times
- Updates dispatch with loading slip number
- Marks dispatch as "loaded"

**Why:** Creates complete audit trail: order → dispatch → loading → delivery

**Step 4: Printing and Handover**
- Generates printable loading slip
- Shows: slip number, vehicle, driver, customer, item list
- Driver signs and takes copy
- Customer copy for delivery proof

**Why:** Provides legal document for material movement

**Technical Info:**
- Methods: `GetAllDispatchForLoadingSlip()`, `GenerateLoadingSlip()` in `OrderDispatchService.cs`
- Tables: `SA_LOADING_SLIP_DETAIL`, `ORDER_DISPATCH_SCHEDULE`, `IP_VEHICLE_TRACK`

---

## 6. Miscellaneous Sub Ledger Setup

### What It Does
Creates additional sub-accounts under main accounting heads for detailed financial tracking. Example: Under "Sundry Debtors", create sub-ledgers for customer categories.

### Why We Need It
- **Problem:** Main accounting heads too broad for detailed analysis
- **Solution:** Create sub-categories for better tracking
- **Benefit:** Detailed financial reports, better cost center tracking, easier audit

### How It Works

**Step 1: Understanding Structure**
- Sub-ledgers organized like tree (parent-child)
- Multiple levels possible (Expenses → Office Expenses → Stationery)
- Unique hierarchical code (e.g., 01.01.01)

**Why:** Easy grouping and analysis of related accounts

**Step 2: Creating New Sub Ledger**
- Select parent sub-ledger (or root level)
- Enter English and Nepali description
- System auto-generates hierarchical code
- Link to main account code
- Set as Group (can have children) or Individual (end node)

**Why:** Maintains consistent numbering, prevents duplicates

**Step 3: Linking to Transactions**
- Select sub-ledger in journal entries
- System posts to both main account and sub-ledger
- Reports show main account summary or sub-ledger details

**Why:** Flexibility in reporting - high-level or detailed view

**Technical Info:**
- Methods: `createNewMiscellaneousSubLedgerSetup()` in `DocumentSetupRepo.cs`
- Table: `FA_SUB_LEDGER_SETUP`
- View: `MiscellaneousSubLedgerSetup.cshtml`

---

# NeoERP Feature Documentation - Part 2

**Author:** [Your Name]  
**Date:** October 2024  
**Purpose:** Knowledge transfer documentation (Business-focused explanations)

---

## Table of Contents - Part 2
7. [Attribute Setup](#7-attribute-setup)
8. [Dealer Setup](#8-dealer-setup)
9. [Account Setup - Nature of Account Logic](#9-account-setup---nature-of-account-logic)
10. [Bank Mapping in Customer & Supplier](#10-bank-mapping-in-customer--supplier)
11. [Item Mapping in Customer & Supplier](#11-item-mapping-in-customer--supplier)
12. [Dealer Mapping in Customer](#12-dealer-mapping-in-customer)

---

## 7. Attribute Setup

### What It Does
Manages product attributes (characteristics) like Color, Size, Brand, etc. These attributes can be assigned to items for better product classification and filtering.

### Why We Need It
- **Problem:** Hard to search and filter products without proper categorization
- **Solution:** Define attributes and assign them to items
- **Benefit:** Better product search, easier inventory management, improved customer experience

### How It Works

**Step 1: Creating Attributes**
- Create attribute groups (e.g., "Color", "Size", "Material")
- Under each group, create specific values (Red, Blue, Green under Color)
- System maintains hierarchical structure

**Why:** Organizes attributes logically, prevents duplicate entries

**Step 2: Assigning Attributes to Items**
- When setting up item, select multiple attributes
- System stores mapping in separate table
- One item can have multiple attributes

**Why:** Flexible system where items can have any combination of attributes

**Step 3: Using Attributes**
- Filter items by attributes in sales/purchase screens
- Reports can group items by attributes
- Helps in inventory analysis (e.g., how much red color items in stock)

**Why:** Makes product data more useful for business decisions

**Technical Info:**
- Methods: `createNewAttributeSetup()`, `insertItemAttributeMapping()` in `DocumentSetupRepo.cs`
- Tables: `IP_ATTRIBUTE_SETUP`, `IP_ITEM_ATTRIBUTE_MAP`
- View: `AttributeSetup.cshtml`

---

## 8. Dealer Setup

### What It Does
Manages dealer/distributor information. Dealers buy in bulk and resell. Tracks dealer details, credit limits, and which customers are linked to which dealer.

### Why We Need It
- **Problem:** Dealers need different pricing, credit terms, and reporting than regular customers
- **Solution:** Separate dealer master with hierarchy and customer mapping
- **Benefit:** Better dealer management, accurate commission calculation, territory-wise sales tracking

### How It Works

**Step 1: Creating Dealer Master**
- Enter dealer code, name (English/Nepali), address
- Set credit limit and credit days (payment terms)
- Link to accounting head for financial posting
- Can create dealer groups and individual dealers under groups

**Why:** Supports dealer hierarchy (Regional → Area → Local Dealers)

**Step 2: Mapping Customers to Dealers**
- In customer setup, link customer to dealer
- Stored in `LINK_SUB_CODE` field
- One dealer can have multiple customers

**Why:** Enables dealer-wise sales reporting and commission calculation

**Step 3: Dealer-Specific Pricing**
- In Rate Schedule, select "Dealer" as party type
- Set different prices for dealers vs regular customers
- System applies correct price based on party type

**Why:** Dealers get wholesale prices, customers get retail prices

**Step 4: Dealer Reports**
- Generate sales reports by dealer
- Shows dealer's direct purchases + sales to their linked customers
- Helps calculate dealer commissions and incentives

**Why:** Complete visibility into dealer business for better relationship management

**Technical Info:**
- Methods: `createNewDealerSetup()`, `GetDealerHierarchicalTree()` in `DocumentSetupRepo.cs`
- Table: `IP_PARTY_TYPE_CODE` (where `PARTY_TYPE_FLAG = 'D'`)
- Customer Link: `LINK_SUB_CODE` in `SA_CUSTOMER_SETUP`
- View: `DealerSetup.cshtml`

---

## 9. Account Setup - Nature of Account Logic

### What It Does
Controls what type of transactions can be posted to an account based on its "nature". For example, a bank account should only allow bank-related transactions.

### Why We Need It
- **Problem:** Users might accidentally post wrong transaction types to accounts
- **Solution:** System enforces rules based on account nature
- **Benefit:** Cleaner accounting data, fewer errors, easier reconciliation

### Account Nature Types

**1. 'AC' - Bank Account**
- **Purpose:** For bank accounts only
- **What it controls:** Only bank-related transactions allowed
- **Why:** Ensures bank reconciliation is accurate
- **Example:** HDFC Bank Account, SBI Current Account

**2. 'CA' - Cash Account**
- **Purpose:** For cash-in-hand accounts
- **What it controls:** Only cash transactions allowed
- **Why:** Separates cash from bank for better control
- **Example:** Cash in Hand, Petty Cash

**3. 'AS' - Asset Account**
- **Purpose:** For fixed assets and investments
- **What it controls:** Asset purchase/sale transactions
- **Why:** Tracks asset lifecycle separately
- **Example:** Machinery, Furniture, Vehicles

**4. 'LI' - Liability Account**
- **Purpose:** For loans and payables
- **What it controls:** Liability-related transactions
- **Why:** Tracks what company owes
- **Example:** Bank Loan, Creditors

**5. 'IN' - Income Account**
- **Purpose:** For revenue accounts
- **What it controls:** Only credit entries (income)
- **Why:** Prevents accidental debit to income accounts
- **Example:** Sales Revenue, Interest Income

**6. 'EX' - Expense Account**
- **Purpose:** For cost accounts
- **What it controls:** Only debit entries (expenses)
- **Why:** Prevents accidental credit to expense accounts
- **Example:** Salary, Rent, Utilities

### How System Uses Nature

**During Transaction Entry:**
- User selects account in voucher entry
- System checks account nature
- If transaction type doesn't match nature, shows error
- Forces user to select correct account

**Why:** Prevents data entry errors at source

**In Bank Mapping:**
- When mapping bank accounts to customers/suppliers
- System only shows accounts with nature = 'AC'
- Cannot accidentally select cash or other account types

**Why:** Ensures only valid bank accounts are mapped

**In Reports:**
- Bank reconciliation report only shows 'AC' nature accounts
- Cash flow report only shows 'CA' nature accounts
- P&L report shows 'IN' and 'EX' nature accounts

**Why:** Reports are accurate and meaningful

**Technical Info:**
- Field: `ACC_NATURE` in `FA_CHART_OF_ACCOUNTS_SETUP` table
- Query Example: `WHERE ACC_NATURE = 'AC'` to get only bank accounts
- Used In: Account setup, bank mapping, transaction entry, all financial reports

---

## 10. Bank Mapping in Customer & Supplier

### What It Does
Stores multiple bank account details for each customer and supplier. When making payments or receiving money, you can select which bank account to use.

### Why We Need It
- **Problem:** Customers/suppliers have multiple bank accounts, need to track which to use
- **Solution:** Store all bank accounts and select appropriate one during payment
- **Benefit:** Faster payment processing, fewer errors, better audit trail

### How It Works

**Step 1: Adding Bank Accounts to Customer**
- In customer setup, there's a "Bank Map" tab
- Add multiple bank accounts
- For each account, store:
  - Bank name
  - Account number
  - Branch name
  - IFSC/SWIFT code
  - Account type (Savings/Current)
  - Whether it's the default account

**Why:** Customer might want payment in different accounts for different purposes

**Step 2: Adding Bank Accounts to Supplier**
- In supplier setup, there's a "Bank Map" tab
- Similar to customer, add multiple bank accounts
- Store complete bank details for each account

**Why:** Ensures we pay suppliers in their preferred accounts

**Step 3: Using Bank Mapping in Transactions**
- When creating payment voucher for supplier
- System shows dropdown of all mapped bank accounts
- User selects appropriate account
- Payment details recorded with bank account reference

**Why:** Provides clear record of which account was used

**Step 4: Bank Account Validation**
- System only allows mapping accounts with nature = 'AC' (Bank Account)
- Cannot map cash accounts or other account types
- Validates account number format

**Why:** Prevents mapping errors, ensures data quality

### Data Storage

**For Customers:**
- Table: `SA_CUSTOMER_BANK_DETAIL` (or similar)
- Key Fields:
  - `CUSTOMER_CODE` - Links to customer
  - `BANK_ACC_CODE` - Links to chart of accounts (nature must be 'AC')
  - `BANK_NAME` - Bank name
  - `ACCOUNT_NO` - Account number
  - `BRANCH_NAME` - Branch details
  - `DEFAULT_FLAG` - Is this the default account

**For Suppliers:**
- Table: `IP_SUPPLIER_BANK_DETAIL` (or similar)
- Key Fields: Same as customer bank mapping

**Technical Info:**
- Nature Check: Always filter `ACC_NATURE = 'AC'` when showing bank accounts
- Multiple Accounts: One customer/supplier can have many bank accounts
- Default Account: Mark one as default for quick selection

---

## 11. Item Mapping in Customer & Supplier

### What It Does
Links specific items to specific customers or suppliers. Controls which items a customer can buy or which items a supplier can provide.

### Why We Need It
- **Problem:** Not all customers should see all products, not all suppliers provide all items
- **Solution:** Map specific items to specific parties
- **Benefit:** Faster order entry, prevents ordering wrong items, better price management

### How It Works

**Step 1: Mapping Items to Customer**
- In customer setup, there's an "Item Map" tab
- Select multiple items from item master
- System stores mapping in `SA_CUSTOMER_ITEM_MAP` table
- One customer can have many items mapped

**Why this helps:**
- **For B2B customers:** They only buy specific products, show only those
- **For dealers:** They handle specific product lines
- **For pricing:** Set customer-specific prices for mapped items only

**Step 2: Mapping Items to Supplier**
- In supplier setup, there's an "Item Map" tab
- Select items that this supplier provides
- System stores in `IP_SUPPLIER_ITEM_MAP` table
- One supplier can supply many items

**Why this helps:**
- **For purchase orders:** Show only relevant suppliers when ordering an item
- **For price comparison:** Compare prices only from suppliers who provide that item
- **For quality tracking:** Track which supplier provided which items

**Step 3: Using Item Mapping in Sales**
- When creating sales order, select customer first
- System checks if customer has item mapping
- If yes, shows only mapped items in dropdown
- If no mapping, shows all items

**Why:** Speeds up order entry, prevents selling wrong items

**Step 4: Using Item Mapping in Purchase**
- When creating purchase order, select item first
- System shows only suppliers who have that item mapped
- Can compare prices from relevant suppliers only

**Why:** Ensures we order from correct suppliers, get best prices

### Data Storage

**Customer Item Mapping:**
- Table: `SA_CUSTOMER_ITEM_MAP`
- Key Fields:
  - `CUSTOMER_CODE` - Links to customer
  - `ITEM_CODE` - Links to item master
  - `COMPANY_CODE` - Company identifier
  - `CREATED_BY` - Who created the mapping
  - `DELETED_FLAG` - Soft delete flag

**Supplier Item Mapping:**
- Table: `IP_SUPPLIER_ITEM_MAP`
- Key Fields: Same structure as customer mapping

### Special Logic

**Updating Mappings:**
- When updating customer/supplier, can modify item list
- System first deletes all existing mappings for that party
- Then inserts new mappings
- Uses bulk insert for better performance

**Why this approach:** Simpler than trying to figure out what changed

**No Mapping = All Items:**
- If no items are mapped to a customer/supplier
- System treats it as "all items allowed"
- Shows complete item list

**Why:** Provides flexibility - mapping is optional, not mandatory

**Technical Info:**
- Tables: `SA_CUSTOMER_ITEM_MAP`, `IP_SUPPLIER_ITEM_MAP`
- Methods: `insertCustomerItemMapping()`, `insertSupplierItemMapping()` in `DocumentSetupRepo.cs`
- Pattern: Delete existing mappings, then bulk insert new ones

---

## 12. Dealer Mapping in Customer

### What It Does
Links customers to dealers, establishing dealer-customer relationship. Helps track which customers are under which dealer's territory for sales tracking and commission calculation.

### Why We Need It
- **Problem:** Need to know which customers are served by which dealer for territory management
- **Solution:** Link each customer to their servicing dealer
- **Benefit:** Accurate dealer performance tracking, correct commission calculation, better territory management

### How It Works

**Step 1: Understanding the Relationship**
- Dealers are created in Dealer Setup (Party Type with flag 'D')
- Customers are created in Customer Setup
- Each customer can be linked to one dealer
- One dealer can have many customers

**Why:** Represents real-world distribution model where dealers serve multiple customers

**Step 2: Linking Customer to Dealer**
- In customer setup form, there's a field for dealer selection
- User selects dealer from dropdown (shows all active dealers)
- System stores dealer's party type code in customer's `LINK_SUB_CODE` field
- This creates the dealer-customer link

**Why this field:** `LINK_SUB_CODE` is a generic field used for various sub-ledger linkages

**Step 3: How System Uses This Mapping**

**For Sales Tracking:**
- When customer places order, system checks their `LINK_SUB_CODE`
- Identifies which dealer this customer belongs to
- Tags the sale to that dealer
- Dealer's sales report includes:
  - Direct sales to dealer
  - Sales to dealer's linked customers

**Why:** Gives complete picture of dealer's business volume

**For Commission Calculation:**
- At month end, system calculates dealer commission
- Includes sales to dealer's linked customers
- Commission rate may differ for direct vs customer sales
- Generates dealer commission report

**Why:** Dealers earn commission on their territory sales, not just direct purchases

**For Territory Management:**
- Management can see which customers are under which dealer
- Can reassign customers to different dealers if needed
- Tracks dealer coverage and gaps

**Why:** Helps in territory planning and dealer performance evaluation

**Step 4: Viewing Dealer Mapping**
- In dealer setup, can view list of linked customers
- Shows customer code, name, location
- Can add/remove customer assignments
- Updates `LINK_SUB_CODE` in customer master

**Why:** Provides dealer-centric view of their customer base

### Data Storage

**Customer Table:**
- Table: `SA_CUSTOMER_SETUP`
- Key Field: `LINK_SUB_CODE`
  - Stores the dealer's party type code
  - If NULL or empty, customer is not linked to any dealer
  - If filled, customer belongs to that dealer's territory

**Dealer Table:**
- Table: `IP_PARTY_TYPE_CODE`
- Key Field: `PARTY_TYPE_CODE`
  - This code is stored in customer's `LINK_SUB_CODE`
  - `PARTY_TYPE_FLAG = 'D'` identifies it as dealer

### Special Scenarios

**Direct Customers (No Dealer):**
- Some customers buy directly from company, not through dealer
- For these, `LINK_SUB_CODE` is kept NULL or empty
- Sales to these customers don't count in any dealer's commission

**Why:** Differentiates between dealer channel and direct sales

**Changing Dealer Assignment:**
- Can update customer's `LINK_SUB_CODE` to different dealer
- Historical sales remain tagged to old dealer
- Future sales go to new dealer
- Useful when territories are reorganized

**Why:** Maintains historical accuracy while allowing flexibility

**Dealer Hierarchy:**
- Can have dealer groups and sub-dealers
- Customer can be linked to sub-dealer
- Reports can roll up to dealer group level

**Why:** Supports multi-level distribution networks

**Technical Info:**
- Customer Field: `LINK_SUB_CODE` in `SA_CUSTOMER_SETUP` table
- Dealer Identification: `PARTY_TYPE_FLAG = 'D'` in `IP_PARTY_TYPE_CODE`
- Methods: `GetCustomerMappingForDealer()`, `getCustomerMapped()` in `DocumentSetupRepo.cs`

---

## Summary

This documentation covers 12 key features implemented in NeoERP:

**Logistics & Dispatch:**
1. Order Dispatch Management - Planning deliveries
2. Gate Entry - Vehicle tracking
3. Loading Slip Generate - Delivery documentation

**Pricing & Discounts:**
4. Rate Schedule - Price management
5. Discount Schedule - Discount management

**Master Data Setup:**
6. Miscellaneous Sub Ledger - Financial sub-accounts
7. Attribute Setup - Product characteristics
8. Dealer Setup - Distributor management

**Advanced Configurations:**
9. Account Nature Logic - Transaction control
10. Bank Mapping - Payment account management
11. Item Mapping - Product-party relationships
12. Dealer Mapping - Territory management

Each feature is designed to solve specific business problems and improve operational efficiency. The documentation focuses on **what** each feature does, **why** it's needed, and **how** it works from a business perspective.

---

**For Technical Details:**
- Refer to the actual code files mentioned in each section
- Database tables are Oracle-based
- Application uses ASP.NET MVC with AngularJS frontend
- All features follow soft-delete pattern (DELETED_FLAG = 'Y')

**Questions?**
Contact the development team before the transition period ends.

---

**Document Version:** 2.0  
**Last Updated:** October 2025

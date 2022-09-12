# Lab Marking Guide ![STATUS](https://img.shields.io/badge/Status-V1.2-green?logo=jekyll)

The marking guide for each section is based on the [Lab Specs](../ReadMe.md) for that section, including the specifications for individual subsystems. Refer to those lab specs for details on what you are to produce, and use these marking guides to determine the general weight breakdown for the deliverable's content.

- [Deliverable 1](./Part-1/Feedback.md)
- [Deliverable 2](./Part-2/Feedback.md)
- [Deliverable 3](./Part-3/Feedback.md)

<!--
## Part A: Team ___

### Project Setup: _____________________

| ✔ |  Item |
|---|-------|
|   | `ReadMe.md` - Team (#/Name), members, task breakdown |
|   | VS Solution |
|   | Entities/DAL - Internal |

### Security Setup: _____________________

| ✔ |  Item |
|---|-------|
|   | Security - Application User + DAL + DatabaseInitilizer (Employees) |
|   | Updated Login/Registration Pages |
|   | Username/Password in App (Default or About) |

### Purchasing Setup: _____________________

| ✔ |  Item |
|---|-------|
|   | Entity Property Renames (see below) |
|   | Custom Classes/Enums: `PurchaseOrderStatusFilter`, `CreditRatingType` |

```csharp
modelBuilder.Entity<Product>()
     // The BillOfMaterials where the product is used...
    .HasMany(e => e.BillOfMaterialsUsage)
    // The product acting as a component...
    .WithRequired(e => e.Component)
    .HasForeignKey(e => e.ComponentID)
    .WillCascadeOnDelete(false);

modelBuilder.Entity<Product>()
    // The BillOfMaterials where the product acts as a "parent"
    .HasMany(e => e.BillOfMaterials)
    // The product acting as a finished product
    .WithOptional(e => e.FinishedProduct)
    .HasForeignKey(e => e.ProductAssemblyID);
modelBuilder.Entity<UnitMeasure>()
    // The products where the unit indicates size
    .HasMany(e => e.SizedProducts)
    // The UnitMeasure for the product's size
    .WithOptional(e => e.SizeUnitMeasure)
    .HasForeignKey(e => e.SizeUnitMeasureCode);

modelBuilder.Entity<UnitMeasure>()
    // The products where the unit indicates weight
    .HasMany(e => e.WeightedProducts)
    // The UnitMeasure for the product's weight
    .WithOptional(e => e.WeightUnitMeasure)
    .HasForeignKey(e => e.WeightUnitMeasureCode);
```

### Staffing Setup: _____________________

| ✔ |  Item |
|---|-------|
|   | Entity Property Renames (see below) |
|   | Custom Classes/Enums: `PayFrequency`, `EmailPromotion`, `PersonType`, `NameStyle` |

```csharp
modelBuilder.Entity<Address>()
    // The SalesOrderHeaders in which this address is the bill-to address
    .HasMany(e => e.BilledSalesOrders)
    // The address to which the SalesOrderHeader is billed
    .WithRequired(e => e.BillToAddress)
    .HasForeignKey(e => e.BillToAddressID)
    .WillCascadeOnDelete(false);

modelBuilder.Entity<Address>()
    // The SalesOrderHeaders in which this address is the ship-to address
    .HasMany(e => e.ShippedSalesOrders)
    // The address to which the SalesOrderHeader is shipped
    .WithRequired(e => e.ShipToAddress)
    .HasForeignKey(e => e.ShipToAddressID)
    .WillCascadeOnDelete(false);
modelBuilder.Entity<Currency>()
    // The CurrencyRates acting as a start point of a conversion
    .HasMany(e => e.OriginatingCurrencyRates)
    // The Currency from which an amount has been converted
    .WithRequired(e => e.FromCurrency)
    .HasForeignKey(e => e.FromCurrencyCode)
    .WillCascadeOnDelete(false);

modelBuilder.Entity<Currency>()
    // The CurrencyRates acting as the endpoint of a conversion
    .HasMany(e => e.FinalCurrencyRates)
    // The Currency to which an amount has been converted
    .WithRequired(e => e.ToCurrency)
    .HasForeignKey(e => e.ToCurrencyCode)
    .WillCascadeOnDelete(false);
```

----

## Part B


### Purchasing: ________________

| ✔ | (Weight) Item |
|---|-------|
|   | (3) User Interace Mockups |
|   | (3) Use Case Diagrms |
|   | (3) Sequence Diagrams |
|   | (3) Class Diagrams |

| Mark | Breakdown |
| ---- | --------- |
| **3** | 3 = Proficient (requirement is met)<br />2 = Capable (requirement is adequately met, minor errors)<br />1 = Limited (requirement is poorly met, major errors)<br />0 = Incomplete (requirement not met, missing large portions) |

### Staffing: ________________

| ✔ | (Weight) Item |
|---|-------|
|   | (3) User Interace Mockups |
|   | (3) Use Case Diagrms |
|   | (3) Sequence Diagrams |
|   | (3) Class Diagrams |

| Mark | Breakdown |
| ---- | --------- |
| **3** | 3 = Proficient (requirement is met)<br />2 = Capable (requirement is adequately met, minor errors)<br />1 = Limited (requirement is poorly met, major errors)<br />0 = Incomplete (requirement not met, missing large portions) |

----

## Part C - Purchasing: ________________

### PO History

| ✔ |  Item |
|---|-------|
|   | Displays PO list with: POId, Vendor, Account #, Status, Rev #, % Complete, Order Date, Subtotal, Tax, Freight, Total, Shipper |
|   | PO List items have links to show order details |
|   | PO List items grouped by Vendor, sorted by vendor (asc) and order date (desc) |
|   | PO List items grouped by Month, sorted by order date (desc) |
|   | PO List items filtered by status: Pending/Approved/Rejected/Complete |
|   | PO List items filter allows All Orders |
|   | Detail page shows PO list header with: POId, Vendor, Account #, Status, Rev #, % Complete, Order Date, Subtotal, Tax, Freight, Total, Shipper, + last modified date |
|   | Detail page shows order items with: product #, name, standard cost, purchasing unit price, order qty, line total |
|   | Detail page's order items tied to `ProductVendor` show: average lead time, standard price, last receipt cost, min/max order quantity, unit measure |
|   | Data presentation has acceptable styling |

### Inventory Managment

| ✔ |  BLL Item |
|---|-----------|
|   | Uses correct BLL signature:<br/>`int SaveOrder(int employeeId, OrderSummary summary, IEnumerable<OrderItem> items)` |
|   | `SaveOrder(...)` handles creation of new orders and edits of existing orders |
|   | `SaveOrder(...)` correctly uses transactions |
|   | Uses correct BLL signature:<br/>`void PlaceOrder(int employeeId, OrderSummary summary, IEnumerable<OrderItem> items)` |
|   | `PlaceOrder(...)` correctly uses transactions |
|   | BLL has method to cancel an order |
|   | Cancel Order correctly uses transactions |
|   | `OrderSummary` class has: `PurchaseOrderId`, `RequiredByDate`, `VendorID`, `ShipMethodID`, `FreightCharge` |
|   | `OrderItem` class has: `ProductID`, `OrderQuantity`, `UnitPrice` |
|   | Save/Place Order correctly handles new items in list by adding to the order |
|   | Save/Place Order correctly handles updating quantities/prices of items already on the order |
|   | Save/Place Order correctly removes items previously saved to the order that are no longer in the list of items passed in to the method |
|   | Save/Place/Cancel order validates that the employee is in the *Purchasing* department |
|   | Save/Place order validates RequiredByDate is in the future by at lease one day. |
|   | Save/Place order validates that products are ones actually sold by that vendor |
|   | Save/Place order applies correct tax rate to the order |
|   | `SaveOrder(...)` validates & sets order status appropriately (`Pending`) |
|   | `SaveOrder(...)` properly modifies the revision number |
|   | `SaveOrder(...)` allows for no items in the order |
|   | `SaveOrder(...)` retains original creation date as the order date |
|   | `PlaceOrder(...)` validates order status (`Pending` or new) and updates to `Approved` |
|   | `PlaceOrder(...)` validates there is at least one item in the order |
|   | `PlaceOrder(...)` validates that item quantities fall within the `ProductVendor` min/max order quantities |
|   | `PlaceOrder(...)` properly modifies the revision number |
|   | `PlaceOrder(...)` properly sets the order date to the current date/time |
|   | `PlaceOrder(...)` validates that none of the items ordered are discontinued items |
|   | Orders can only be cancelled if `Pending` or `Approved` |
|   | Only `Pending` orders can be modified |
|   | `Approved` orders can be viewed |

| ✔ |  UI Item |
|---|-----------|
|   | Vendor selection shows vendor's name, account #, credit rating, is preferred vendor, and list of contacts |
|   | Only active vendors available for selection |
|   | Products are only those available through that vendor |
|   | Internally built products are not listed |
|   | Available products show name, product #, standard cost, and list price |
|   | Available product details show name, product #, color (if any), safety stock level, reorder point, standard cost, list price, size & units, weight & units, product line, class, style, product subcategory, product model |
|   | Purchasing save/place is performed as a bulk order entry |
|   | Warning notifications re: no items |
|   | Warning notifications re: min/max quantity problems |
|   | Warning notifications re: discontinued items |
|   | Errors show as a group |
|   | Error details include PO Id and current order status |
|   | Layout & UX is user-friendly |
|   | Appropriate styling used |

## Part C - Staffing: ________________

### Staff Directory

| ✔ | Item |
|---|------|
|   | Displays Staff Directory with full name (`last, first`), job title, hire date, vacation hours, sick leave hours, is salaried |
|   | Staff directory have link to show personal details |
|   | Staff directory grouped by department, sorted by last/first name |
|   | Staff directory grouped by shift, sorted by last/first name |
|   | Filter for partial first/last name |
|   | Only shows currently employed staff |
|   | Department group shows shift name in details |
|   | Shift group shows department name in details |
|   | Details show complete name (first, middle, and last) and contact info (home address, emails, phone numbers & phone number types) |
|   | Data presentation has acceptable styling |

### Hire Employee

| ✔ |  BLL Item |
|---|-------|
|   | Uses correct BLL signature:<br/>`string HireEmployee(int managerId, Applicant newHire, JobPosition, position, DateTime startDate)` |
|   | `HireEmployee(...)` correctly uses transactions |
|   | `Applicant` class has: `FirstName`, `LastName`, `MiddleName`, `Suffix`, `Title`, `MaritalStatus`, `Gender`, `DateOfBirth`, `PersonType`, `HomeAddress:Address`, `HomeContact:ContactInformation` |
|   | `Address` class has: `AddressLine1`, `AddressLine2`, `City`, `PostalCode` |
|   | `ContactInformation` class has: `Email`, `PhoneNumber`, `PhoneType` |
|   | `JobPosition` class has: `PositionTitle`, `DepartmentID`, `ShiftID`, `PayRate`, `IsSalariedPosition` |
|   | `HireEmployee(...)` validates applicant is 18 or older |
|   | `HireEmployee(...)` validates job position and department combination already in use/exist |
|   | `HireEmployee(...)` validates shift |
|   | `HireEmployee(...)` validates StartDate as first Monday after current date |
|   | `HireEmployee(...)` validates StartDate as no later than one month after current date |
|   | `HireEmployee(...)` validates StartDate as regular work day (Mon-Fri) |
|   | `HireEmployee(...)` assigns a unique NationalIDNumber |
|   | `HireEmployee(...)` assigns a unique login id of `adventure-works\firstname#` |
|   | `HireEmployee(...)` assigns unique work email of `firstname#@adventure-works.com` |
|   | `HireEmployee(...)` assigns unique work phone of `122-555-####` |
|   | `HireEmployee(...)` sets bi-weekly if salaried; otherwise monthly |
|   | `HireEmployee(...)` sets vacation/sick leave hours of zero |
|   | `HireEmployee(...)` creates `SalesPerson` if job position belongs to `Sales` group |
|   | `HireEmployee(...)` uses a commission of `0.01` for `SalesPerson` |
|   | `HireEmployee(...)` validates daytime shift if creating a `SalesPerson` |
|   | `HireEmployee(...)` ignores `Address.Location` and `Employee.OrganizationNode` |

| ✔ |  UI Item |
|---|-----------|
|   | Successful hires show National ID Number, assigned Login ID, work email, work phone |
|   | Personal titles of `Mr.`, `Mrs.`, `Ms.` and `none` supported |
|   | Job position uses cascading drop-downs for selection |
|   | Department Group selection populates Department drop-down |
|   | Department selection populates Job Position drop-down |
|   | Errors show as a group |
|   | Layout & UX is user-friendly |
|   | Appropriate styling is used |
-->

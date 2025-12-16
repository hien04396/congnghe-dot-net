## Running the OnlineShop ASP.NET Core MVC App

### 1. Prerequisites

- .NET SDK 9 installed (check with `dotnet --version`)
- MySQL server running locally on `localhost:3306`
  - User: `root`
  - Password: `root`

### 2. Project structure

Key folders:

- `OnlineShop/` – ASP.NET Core MVC project
- `OnlineShop/Data/` – EF Core DbContext and seeding
- `OnlineShop/Models/` – Entity classes (products, categories, orders, etc.)
- `OnlineShop/Controllers/` – MVC controllers (storefront, admin, cart, orders)
- `OnlineShop/Views/` – Razor views
- `db/online_store_schema.sql` – Generated schema SQL
- `db/online_store_seed.sql` – Sample data SQL

### 3. Database setup

The app is configured (in `OnlineShop/appsettings.json`) to connect with:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=online_store;User=root;Password=root;"
  }
}
```

You have two options:

#### Option A – Let EF migrations create the DB (recommended)

From the project root:

```bash
cd "OnlineShop"
dotnet ef database update
```

> Note: The app also runs `DbInitializer.EnsureSeedData` at startup, so initial admin and sample products are seeded automatically.

#### Option B – Use SQL scripts manually

From a MySQL client (Workbench, CLI, etc.):

1. Run schema:

   ```sql
   SOURCE /path/to/project/db/online_store_schema.sql;
   ```

2. Run seed:

   ```sql
   SOURCE /path/to/project/db/online_store_seed.sql;
   ```

Make sure the database name is `online_store`.

### 4. Restore packages and build

From the project root:

```bash
cd "OnlineShop"
dotnet restore
dotnet build
```

### 5. Run the application

From `OnlineShop/`:

```bash
dotnet run
```

By default the app will listen on `http://localhost:5000` (and HTTPS on `https://localhost:5001` if enabled). Check the console output for the exact URLs.

### 6. How to use the app

- **Storefront (customers):**
  - Home / products: `http://localhost:5000/`
  - Browse products: `http://localhost:5000/Store/Products`
  - Categories list: `http://localhost:5000/Store/Categories`
  - Cart: `http://localhost:5000/Cart`
  - Login/Register: `http://localhost:5000/Account/Login`, `http://localhost:5000/Account/Register`
  - Orders history: `http://localhost:5000/Orders`

- **Admin area:**
  - Admin login: `http://localhost:5000/admin`
  - After login, dashboard shows quick stats and links.

### 7. Default accounts and behavior

- **Admin:**
  - Username: `admin`
  - Password: `admin`
  - This account is hard-coded and also stored in the `AdminUsers` table on first use.

- **Sample customers (via seed SQL):**
  - `alice` / `alice123`
  - `bob` / `bob123`

You can also register new customer accounts from the UI; passwords are stored as plain text (for demo/class purposes only).

### 8. Features checklist

- Admin:
  - CRUD for: product categories, products, product images, product inventory, product reviews (fake reviews), product promos (amount off per product), admin accounts.
  - Admin login at `/admin` with simple form.

- Storefront:
  - Browse categories from admin-defined list.
  - Product listing with pagination and search by name.
  - Product details with images, description, price, promo, and reviews.
  - Session-based cart with stock checks and “out of stock” display.
  - Customers must register/login to checkout; browsing is allowed without login.
  - Checkout removes out-of-stock items automatically and creates orders.
  - Customers can view their order history.

### 9. Notes / limitations

- Passwords are stored in plain text for both admin and customer accounts (not for production).
- File uploads are not implemented; product images use URLs/paths typed by admin.
- UI is intentionally simple and “student-style” with basic Bootstrap layout.



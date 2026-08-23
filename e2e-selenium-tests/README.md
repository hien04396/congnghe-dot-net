# E2E Selenium Tests

Bộ test tự động cho **trang Admin** và **trang khách hàng** của Online Shop.

Công nghệ:

- Java
- Selenium WebDriver
- JUnit 5

## Cấu trúc thư mục

```
e2e-selenium-tests/
├── pom.xml
├── README.md
└── src/test/java/tests/
    ├── common/                      # Dùng chung
    │   ├── CauHinh.java             # Địa chỉ website, mở Chrome, helper
    │   ├── InKetQua.java            # Mở/đóng Chrome, in PASSED / FAILED
    │   └── InKetQuaAdmin.java       # InKetQua + đăng nhập Admin
    ├── admin/                       # Test trang Admin
    │   ├── LoginTest.java
    │   ├── DashboardTest.java
    │   ├── CategoryTest.java
    │   ├── ProductTest.java
    │   ├── AdminUserTest.java
    │   ├── InventoryTest.java
    │   ├── ProductImageTest.java
    │   ├── PromoTest.java
    │   └── ReviewTest.java
    └── user/                        # Test trang khách hàng
        ├── HomeTest.java
        ├── CustomerAccountTest.java
        ├── ShopTest.java
        ├── CartTest.java
        └── OrderTest.java
```

## Chuẩn bị

1. Cài **Java 17+**, **Maven**, **Google Chrome**.
2. Chạy website Online Shop trước khi test.

Mặc định test dùng `http://127.0.0.1:5000` (đúng với `launchSettings.json`).

Trên macOS, cổng 5000 thường bị AirPlay chiếm. Hãy chạy website bằng:

```bash
cd OnlineShop
dotnet run --urls http://127.0.0.1:5088 --no-launch-profile
```

Rồi chạy test:

```bash
cd e2e-selenium-tests
mvn test -DbaseUrl=http://127.0.0.1:5088
```

## Cách chạy test

Trong thư mục `e2e-selenium-tests`:

```bash
mvn test
```

Chạy **chỉ test Admin**:

```bash
mvn test -Dtest=tests.admin.*
```

Chạy **chỉ test trang khách hàng**:

```bash
mvn test -Dtest=tests.user.*
```

Chạy 1 class:

```bash
mvn test -Dtest=LoginTest
```

## Nhìn thấy trình duyệt khi test

Mở file `src/test/java/tests/common/CauHinh.java` và **xóa** dòng:

```java
options.addArguments("--headless=new");
```

## Tài khoản dùng để test

Admin:

- Username: `admin`
- Password: `admin`

Khách hàng: test tự đăng ký tài khoản mới (không dùng sẵn một user cố định).

## Cách đọc một test

Mỗi test class **kế thừa** `InKetQua`. Class cha tự làm 2 việc:

1. Mở Chrome trước mỗi test
2. Đóng Chrome và in PASSED / FAILED sau mỗi test

Trong từng test chỉ còn: vào trang, thao tác, `assertTrue` / `assertFalse`.

Test Admin đã đăng nhập thì `extends InKetQuaAdmin`.
`LoginTest` và `DashboardTest` chỉ `extends InKetQua`.

Khi chạy `mvn test`, Terminal in kết quả từng test:

```
========================================
TEST: Đăng nhập thành công
RESULT: PASSED
========================================
```

# E2E Selenium Tests

Test tự động trang Admin và trang khách hàng.

Công nghệ: Java, Selenium WebDriver, JUnit 5

## Cấu trúc

```
e2e-selenium-tests/
├── pom.xml
├── README.md
└── src/test/java/tests/
    ├── common/
    │   ├── CauHinh.java
    │   ├── InKetQua.java
    │   └── InKetQuaAdmin.java
    ├── admin/
    └── user/
```

## Chuẩn bị

1. Cài Java 17+, Maven, Google Chrome
2. Chạy website Online Shop trước khi test

Mặc định test dùng `http://127.0.0.1:5000`.

Đổi địa chỉ:

```bash
mvn test -DbaseUrl=http://127.0.0.1:5088
```

## Cách chạy

Trong thư mục `e2e-selenium-tests`:

```bash
mvn test
```

Chỉ test Admin:

```bash
mvn test -Dtest=tests.admin.*
```

Chỉ test khách hàng:

```bash
mvn test -Dtest=tests.user.*
```

Một class:

```bash
mvn test -Dtest=LoginTest
```

## Tài khoản

Admin: `admin` / `admin`

Khách hàng: test tự đăng ký tài khoản mới.

Kết quả trên Terminal:

```
========================================
TEST: Đăng nhập thành công
RESULT: PASSED
========================================
```

package tests.user;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test đăng ký, đăng nhập, đăng xuất của khách hàng.
 */
public class CustomerAccountTest extends InKetQua {

    @Test
    @DisplayName("Đăng ký tài khoản khách hàng thành công")
    void testRegisterSuccessfully() {
        String username = "user" + System.currentTimeMillis();

        // Mở trang đăng ký
        driver.get(CauHinh.BASE_URL + "/Account/Register");

        // Nhập thông tin
        driver.findElement(By.id("Username")).sendKeys(username);
        driver.findElement(By.id("Password")).sendKeys("123456");
        driver.findElement(By.id("ConfirmPassword")).sendKeys("123456");

        // Nhấn Register
        CauHinh.bamNut(driver, "Register");

        // Kiểm tra đã vào trang chủ và thấy tên tài khoản
        driver.findElement(By.xpath("//*[contains(text(),'Hello, " + username + "')]"));
        assertTrue(driver.getPageSource().contains("Hello, " + username));
        assertTrue(driver.getPageSource().contains("Welcome to our online store"));
        driver.findElement(By.linkText("Orders"));
    }

    @Test
    @DisplayName("Đăng ký khi xác nhận mật khẩu không khớp")
    void testRegisterWithMismatchedPassword() {
        // Mở trang đăng ký
        driver.get(CauHinh.BASE_URL + "/Account/Register");

        // Nhập password khác Confirm password
        driver.findElement(By.id("Username")).sendKeys("user" + System.currentTimeMillis());
        driver.findElement(By.id("Password")).sendKeys("123456");
        driver.findElement(By.id("ConfirmPassword")).sendKeys("654321");

        // Nhấn Register
        CauHinh.bamNut(driver, "Register");

        // Vẫn ở trang đăng ký
        assertTrue(driver.getCurrentUrl().contains("Register"));
        assertTrue(driver.getPageSource().contains("Customer Register"));
    }

    @Test
    @DisplayName("Đăng nhập khách hàng thành công")
    void testCustomerLoginSuccessfully() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký trước để có tài khoản
        CauHinh.dangKyKhach(driver, username, "123456");

        // Đăng xuất
        CauHinh.dangXuatKhach(driver);

        // Đăng nhập lại
        driver.get(CauHinh.BASE_URL + "/Account/Login");
        driver.findElement(By.id("Username")).sendKeys(username);
        driver.findElement(By.id("Password")).sendKeys("123456");
        CauHinh.bamNut(driver, "Login");

        // Kiểm tra đăng nhập thành công
        driver.findElement(By.xpath("//*[contains(text(),'Hello, " + username + "')]"));
        assertTrue(driver.getPageSource().contains("Hello, " + username));
    }

    @Test
    @DisplayName("Đăng nhập khách hàng sai password")
    void testCustomerLoginWithWrongPassword() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký trước
        CauHinh.dangKyKhach(driver, username, "123456");
        CauHinh.dangXuatKhach(driver);

        // Đăng nhập sai password
        driver.get(CauHinh.BASE_URL + "/Account/Login");
        driver.findElement(By.xpath("//h2[contains(text(),'Customer Login')]"));
        driver.findElement(By.id("Username")).sendKeys(username);
        driver.findElement(By.id("Password")).sendKeys("sai-mat-khau");
        CauHinh.bamNut(driver, "Login");

        // Đợi thông báo lỗi hiện ra (trang cần tải lại sau khi gửi form)
        driver.findElement(By.xpath("//*[contains(.,'Invalid username or password')]"));
        assertTrue(driver.getCurrentUrl().contains("Account/Login"));
        assertFalse(driver.getPageSource().contains("Hello, " + username));
    }

    @Test
    @DisplayName("Đăng ký khi username đã tồn tại")
    void testRegisterWithDuplicateUsername() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký lần 1 thành công
        CauHinh.dangKyKhach(driver, username, "123456");
        CauHinh.dangXuatKhach(driver);

        // Đăng ký lại cùng username
        driver.get(CauHinh.BASE_URL + "/Account/Register");
        driver.findElement(By.id("Username")).sendKeys(username);
        driver.findElement(By.id("Password")).sendKeys("123456");
        driver.findElement(By.id("ConfirmPassword")).sendKeys("123456");
        CauHinh.bamNut(driver, "Register");

        // Vẫn ở trang đăng ký, báo username đã được dùng
        driver.findElement(By.xpath("//*[contains(.,'Username is already taken')]"));
        assertTrue(driver.getCurrentUrl().contains("Register"));
        assertTrue(driver.getPageSource().contains("Username is already taken"));
    }

    @Test
    @DisplayName("Đăng xuất khách hàng thành công")
    void testCustomerLogout() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký (đã đăng nhập)
        CauHinh.dangKyKhach(driver, username, "123456");

        // Nhấn Logout
        CauHinh.dangXuatKhach(driver);

        // Kiểm tra đã về trang chủ và thấy nút Login
        driver.findElement(By.linkText("Login"));
        assertTrue(driver.getPageSource().contains("Login"));
        assertTrue(driver.getPageSource().contains("Register"));
        assertFalse(driver.getPageSource().contains("Hello, " + username));
    }
}

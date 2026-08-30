package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test chức năng đăng nhập Admin.
 */
public class LoginTest extends InKetQua {

    @Test
    @DisplayName("Đăng nhập thành công")
    void testLoginSuccessfully() {
        // Mở trang đăng nhập Admin
        driver.get(CauHinh.BASE_URL + "/admin");

        // Nhập username
        driver.findElement(By.id("Username")).sendKeys("admin");

        // Nhập password
        driver.findElement(By.id("Password")).sendKeys("admin");

        // Nhấn nút Login
        driver.findElement(By.cssSelector("button[type='submit']")).click();

        // Đợi trang Dashboard hiện ra
        driver.findElement(By.xpath("//h2[contains(text(),'Admin Dashboard')]"));

        // Kiểm tra đã vào Dashboard
        assertTrue(driver.getCurrentUrl().contains("AdminDashboard"));
        assertTrue(driver.getPageSource().contains("Admin Dashboard"));
    }

    @Test
    @DisplayName("Đăng nhập sai password")
    void testLoginWithWrongPassword() {
        // Mở trang đăng nhập Admin
        driver.get(CauHinh.BASE_URL + "/admin");

        // Nhập username đúng
        driver.findElement(By.id("Username")).sendKeys("admin");

        // Nhập password sai
        driver.findElement(By.id("Password")).sendKeys("sai-mat-khau");

        // Nhấn nút Login
        driver.findElement(By.cssSelector("button[type='submit']")).click();

        // Đợi thông báo lỗi hiện ra
        driver.findElement(By.xpath("//*[contains(text(),'Invalid username or password')]"));

        // Kiểm tra vẫn ở trang login
        assertTrue(driver.getCurrentUrl().contains("/admin"));
        assertFalse(driver.getCurrentUrl().contains("AdminDashboard"));
        assertTrue(driver.getPageSource().contains("Invalid username or password"));
    }

    @Test
    @DisplayName("Đăng nhập khi để trống username")
    void testLoginWithEmptyUsername() {
        // Mở trang đăng nhập Admin
        driver.get(CauHinh.BASE_URL + "/admin");

        // Không nhập username, chỉ nhập password
        driver.findElement(By.id("Password")).sendKeys("admin");

        // Nhấn nút Login
        driver.findElement(By.cssSelector("button[type='submit']")).click();

        // Kiểm tra chưa vào được Dashboard
        assertFalse(driver.getCurrentUrl().contains("AdminDashboard"));
        assertTrue(driver.getCurrentUrl().contains("/admin"));
    }

    @Test
    @DisplayName("Đã đăng nhập thì GET /admin chuyển về Dashboard")
    void testRedirectToDashboardWhenAlreadyLoggedIn() {
        CauHinh.dangNhapAdmin(driver);
        driver.get(CauHinh.BASE_URL + "/admin");

        driver.findElement(By.xpath("//h2[contains(text(),'Admin Dashboard')]"));
        assertTrue(driver.getCurrentUrl().contains("AdminDashboard"));
    }
}

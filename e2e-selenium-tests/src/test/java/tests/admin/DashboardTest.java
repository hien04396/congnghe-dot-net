package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test trang Dashboard và đăng xuất.
 */
public class DashboardTest extends InKetQua {

    @Test
    @DisplayName("Dashboard hiển thị sau khi đăng nhập")
    void testDashboardShowsAfterLogin() {
        // Đăng nhập
        CauHinh.dangNhapAdmin(driver);

        // Đợi Dashboard hiện ra
        driver.findElement(By.xpath("//h2[contains(text(),'Admin Dashboard')]"));

        // Kiểm tra các thông tin trên Dashboard
        assertTrue(driver.getCurrentUrl().contains("AdminDashboard"));
        assertTrue(driver.getPageSource().contains("Products"));
        assertTrue(driver.getPageSource().contains("Categories"));
        assertTrue(driver.getPageSource().contains("Orders"));
    }

    @Test
    @DisplayName("Không vào được Dashboard khi chưa đăng nhập")
    void testCannotOpenDashboardWithoutLogin() {
        // Mở thẳng trang Dashboard, chưa đăng nhập
        driver.get(CauHinh.BASE_URL + "/AdminDashboard");

        // Hệ thống phải chuyển về trang login
        assertTrue(driver.getCurrentUrl().contains("/admin"));
        assertTrue(driver.getPageSource().contains("Admin Login"));
        assertFalse(driver.getPageSource().contains("Admin Dashboard"));
    }

    @Test
    @DisplayName("Đăng xuất thành công")
    void testLogout() {
        // Đăng nhập
        CauHinh.dangNhapAdmin(driver);

        // Nhấn nút Logout
        driver.findElement(By.xpath("//button[contains(text(),'Logout')]")).click();

        // Đợi trang login hiện ra
        driver.findElement(By.xpath("//h2[contains(text(),'Admin Login')]"));

        // Kiểm tra quay về trang login
        assertTrue(driver.getCurrentUrl().contains("/admin"));
        assertTrue(driver.getPageSource().contains("Admin Login"));
    }
}

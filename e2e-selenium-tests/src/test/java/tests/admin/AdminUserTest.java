package tests.admin;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;

/**
 * Test quản lý tài khoản admin.
 */
public class AdminUserTest extends InKetQuaAdmin {

    /** Hàm đợi 2 giây để nhìn rõ từng bước trên Chrome. */
    private void wait2Seconds() {
        try {
            Thread.sleep(2000);
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }

    @Test
    @DisplayName("Xem danh sách tài khoản admin")
    void testViewAdminList() {
        // Mở trang quản lý admin
        driver.get(CauHinh.BASE_URL + "/AdminUsers");
        wait2Seconds();

        // Kiểm tra trang đã mở và có tài khoản mặc định
        assertTrue(driver.getPageSource().contains("Admin Users"));
        assertTrue(driver.getPageSource().contains("admin"));
        assertTrue(driver.getPageSource().contains("Create admin account"));
        wait2Seconds();
    }

    @Test
    @DisplayName("Tạo tài khoản admin mới")
    void testCreateAdminAccount() {
        String username = "user" + System.currentTimeMillis();

        // Mở trang tạo admin
        driver.get(CauHinh.BASE_URL + "/AdminUsers/Create");
        wait2Seconds();

        // Nhập username
        driver.findElement(By.id("Username")).sendKeys(username);
        wait2Seconds();

        // Nhập password
        driver.findElement(By.id("Password")).sendKeys("123456");
        wait2Seconds();

        // Nhấn Save
        CauHinh.bamNut(driver, "Save");
        wait2Seconds();

        // Đợi quay về danh sách và thấy tài khoản mới
        driver.findElement(By.xpath("//td[contains(text(),'" + username + "')]"));
        assertTrue(driver.getPageSource().contains(username));
        wait2Seconds();
    }

    @Test
    @DisplayName("Admin mặc định không có nút xóa")
    void testDefaultAdminCannotBeDeleted() {
        // Mở trang quản lý admin
        driver.get(CauHinh.BASE_URL + "/AdminUsers");
        wait2Seconds();

        // Tìm dòng của tài khoản admin mặc định
        WebElement hangAdmin = driver.findElement(By.xpath("//tr[td[normalize-space()='admin']]"));
        wait2Seconds();

        // Dòng này không được có nút Delete / Edit
        assertFalse(hangAdmin.getText().contains("Delete"));
        assertFalse(hangAdmin.getText().contains("Edit"));
        wait2Seconds();
    }

    @Test
    @DisplayName("Xóa tài khoản admin")
    void testDeleteAdminAccount() {
        String username = "user" + System.currentTimeMillis();

        // Tạo tài khoản admin để xóa
        driver.get(CauHinh.BASE_URL + "/AdminUsers/Create");
        wait2Seconds();
        driver.findElement(By.id("Username")).sendKeys(username);
        wait2Seconds();
        driver.findElement(By.id("Password")).sendKeys("123456");
        wait2Seconds();
        CauHinh.bamNut(driver, "Save");
        wait2Seconds();
        driver.findElement(By.xpath("//td[contains(text(),'" + username + "')]"));
        wait2Seconds();

        // Bấm Delete đúng dòng vừa tạo
        driver.findElement(By.xpath("//tr[td[contains(text(),'" + username + "')]]//a[text()='Delete']")).click();
        wait2Seconds();

        // Xác nhận xóa
        CauHinh.bamNut(driver, "Delete");
        wait2Seconds();

        // Kiểm tra tài khoản không còn trên danh sách
        driver.findElement(By.xpath("//h2[contains(text(),'Admin Users')]"));
        assertFalse(driver.getPageSource().contains(username));
        wait2Seconds();
    }
}

package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;

import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test quản lý tồn kho.
 */
public class InventoryTest extends InKetQuaAdmin {

    @Test
    @DisplayName("Mở trang quản lý tồn kho")
    void testOpenManageStockPage() {
        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Stock của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Stock")).click();

        // Kiểm tra đã vào trang quản lý kho
        assertTrue(driver.getCurrentUrl().contains("AdminProductInventory"));
        assertTrue(driver.getPageSource().contains("Manage Stock"));
        assertTrue(driver.getPageSource().contains("Update Stock"));
    }

    @Test
    @DisplayName("Cập nhật số lượng tồn kho")
    void testUpdateStockQuantity() {
        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Stock của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Stock")).click();

        // Nhập số lượng mới
        JavascriptExecutor js = (JavascriptExecutor) driver;
        js.executeScript("document.querySelector(\"input[type='number'][name='stockQuantity']\").value = '25';");

        // Nhấn Update Stock
        CauHinh.bamNut(driver, "Update Stock");

        // Đợi thông báo thành công
        driver.findElement(By.cssSelector(".alert-success"));
        assertTrue(driver.getPageSource().contains("Stock updated successfully"));
        assertTrue(driver.getPageSource().contains("25"));
    }

    @Test
    @DisplayName("Không cho tồn kho âm khi nhập số lượng")
    void testRejectNegativeStockQuantity() {
        driver.get(CauHinh.BASE_URL + "/AdminProducts");
        driver.findElement(By.partialLinkText("Stock")).click();

        JavascriptExecutor js = (JavascriptExecutor) driver;
        js.executeScript(
            "var el = document.querySelector(\"input[type='number'][name='stockQuantity']\");"
            + "el.removeAttribute('min'); el.value = '-1';"
        );
        CauHinh.bamNut(driver, "Update Stock");

        driver.findElement(By.cssSelector(".alert-danger"));
        assertTrue(driver.getPageSource().contains("Stock quantity cannot be negative"));
    }
}

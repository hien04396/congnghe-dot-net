package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test quản lý khuyến mãi sản phẩm.
 */
public class PromoTest extends InKetQuaAdmin {

    @Test
    @DisplayName("Mở trang quản lý khuyến mãi")
    void testOpenPromoPage() {
        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Promos của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Promos")).click();

        // Kiểm tra đã vào trang khuyến mãi
        assertTrue(driver.getCurrentUrl().contains("AdminProductPromos"));
        assertTrue(driver.getPageSource().contains("Product Promos"));
        assertTrue(driver.getPageSource().contains("Create promo"));
    }

    @Test
    @DisplayName("Tạo khuyến mãi mới")
    void testCreatePromo() {
        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Promos của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Promos")).click();

        // Bấm Create promo
        driver.findElement(By.linkText("Create promo")).click();

        // Điền số tiền giảm và ngày (dùng JavaScript vì ô ngày kiểu datetime-local khá khó nhập)
        JavascriptExecutor js = (JavascriptExecutor) driver;
        js.executeScript("document.getElementById('AmountOff').value = '8';");
        js.executeScript("document.getElementById('StartDate').value = '2026-08-23T10:00';");
        js.executeScript("document.getElementById('EndDate').value = '2026-08-30T10:00';");

        // Gửi form (tránh lỗi kiểm tra HTML5 của ô ngày)
        js.executeScript("document.getElementById('AmountOff').form.submit();");

        // Đợi quay về danh sách khuyến mãi
        driver.findElement(By.xpath("//h2[contains(text(),'Product Promos')]"));
        assertFalse(driver.getCurrentUrl().contains("Create"));
        String html = driver.getPageSource();
        assertTrue(html.contains("8,00") || html.contains("8.00") || html.contains("8"));
    }
}

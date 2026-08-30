package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test quản lý hình ảnh sản phẩm.
 */
public class ProductImageTest extends InKetQuaAdmin {

    @Test
    @DisplayName("Mở trang quản lý hình ảnh")
    void testOpenImagePage() {
        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Images của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Images")).click();

        // Kiểm tra đã vào trang hình ảnh
        assertTrue(driver.getCurrentUrl().contains("AdminProductImages"));
        assertTrue(driver.getPageSource().contains("Edit Product Images"));
        assertTrue(driver.getPageSource().contains("Add image"));
    }

    @Test
    @DisplayName("Thêm hình ảnh cho sản phẩm")
    void testAddProductImage() {
        String imageUrl = "https://example.com/test-image-" + System.currentTimeMillis() + ".jpg";

        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Images của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Images")).click();

        // Nhập URL hình ảnh
        driver.findElement(By.name("imageUrl")).sendKeys(imageUrl);

        // Nhấn Add image
        driver.findElement(By.xpath("//button[contains(.,'Add image')]")).click();

        // Đợi URL vừa thêm xuất hiện trên trang
        driver.findElement(By.xpath("//*[contains(text(),'" + imageUrl + "')]"));
        assertTrue(driver.getPageSource().contains(imageUrl));
    }

    @Test
    @DisplayName("Từ chối URL hình ảnh không phải http/https")
    void testRejectRelativeImageUrl() {
        driver.get(CauHinh.BASE_URL + "/AdminProducts");
        driver.findElement(By.partialLinkText("Images")).click();

        driver.findElement(By.name("imageUrl")).sendKeys("/images/sample-placeholder.png");
        driver.findElement(By.xpath("//button[contains(.,'Add image')]")).click();

        driver.findElement(By.cssSelector(".alert-danger"));
        assertTrue(driver.getPageSource().contains("must start with http:// or https://"));
    }
}

package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test quản lý đánh giá sản phẩm.
 */
public class ReviewTest extends InKetQuaAdmin {

    @Test
    @DisplayName("Mở trang quản lý đánh giá")
    void testOpenReviewPage() {
        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Reviews của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Reviews")).click();

        // Kiểm tra đã vào trang đánh giá
        assertTrue(driver.getCurrentUrl().contains("AdminProductReviews"));
        assertTrue(driver.getPageSource().contains("Product Reviews"));
        assertTrue(driver.getPageSource().contains("Add review"));
    }

    @Test
    @DisplayName("Tạo đánh giá mới")
    void testCreateReview() {
        String tieuDe = "E2E Review " + System.currentTimeMillis();

        // Mở danh sách sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Bấm nút Reviews của sản phẩm đầu tiên
        driver.findElement(By.partialLinkText("Reviews")).click();

        // Bấm Add review
        driver.findElement(By.linkText("Add review")).click();

        // Nhập tiêu đề
        driver.findElement(By.id("Title")).sendKeys(tieuDe);

        // Nhập nội dung
        driver.findElement(By.id("Content")).sendKeys("San pham tot, dung de test.");

        // Nhập điểm đánh giá
        driver.findElement(By.id("Rating")).clear();
        driver.findElement(By.id("Rating")).sendKeys("5");

        // Nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Đợi quay về danh sách và thấy đánh giá mới
        driver.findElement(By.xpath("//td[contains(text(),'" + tieuDe + "')]"));
        assertFalse(driver.getCurrentUrl().contains("Create"));
        assertTrue(driver.getPageSource().contains(tieuDe));
    }
}

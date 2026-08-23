package tests.user;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.time.Duration;

import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test xem sản phẩm và danh mục trên web khách hàng.
 */
public class ShopTest extends InKetQua {

    @Test
    @DisplayName("Xem danh sách sản phẩm")
    void testViewProductList() {
        // Mở trang Shop
        driver.get(CauHinh.BASE_URL + "/Store/Products");

        // Kiểm tra trang sản phẩm
        assertTrue(driver.getPageSource().contains("Products"));
        assertTrue(driver.getPageSource().contains("View details"));
        assertTrue(driver.findElements(By.linkText("View details")).size() > 0);
    }

    @Test
    @DisplayName("Tìm kiếm sản phẩm")
    void testSearchProduct() {
        // Mở trang Shop
        driver.get(CauHinh.BASE_URL + "/Store/Products");

        // Nhập từ khóa tìm kiếm
        driver.findElement(By.name("search")).sendKeys("Shirt");

        // Nhấn Filter
        CauHinh.bamNut(driver, "Filter");

        // Đợi URL có search
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.urlContains("search"));

        // Kiểm tra kết quả
        assertTrue(driver.getCurrentUrl().contains("search"));
        assertTrue(driver.getPageSource().contains("Shirt"));
    }

    @Test
    @DisplayName("Xem chi tiết sản phẩm")
    void testViewProductDetails() {
        // Mở trang Shop
        driver.get(CauHinh.BASE_URL + "/Store/Products");

        // Bấm View details của sản phẩm đầu tiên
        driver.findElement(By.linkText("View details")).click();

        // Kiểm tra trang chi tiết
        assertTrue(driver.getCurrentUrl().contains("Store/Details"));
        assertTrue(driver.getPageSource().contains("Add to cart")
                || driver.getPageSource().contains("Out of stock"));
        assertTrue(driver.getPageSource().contains("Reviews"));
    }

    @Test
    @DisplayName("Xem danh mục sản phẩm")
    void testViewCategories() {
        // Mở trang danh mục
        driver.get(CauHinh.BASE_URL + "/Store/Categories");

        // Kiểm tra trang danh mục
        assertTrue(driver.getPageSource().contains("Product Categories"));
        assertTrue(driver.getPageSource().contains("View products"));

        // Bấm View products của danh mục đầu tiên
        driver.findElement(By.linkText("View products")).click();
        assertTrue(driver.getCurrentUrl().contains("Store/Products"));
        assertTrue(driver.getPageSource().contains("Products"));
    }

    @Test
    @DisplayName("Lọc sản phẩm theo danh mục")
    void testFilterProductByCategory() {
        // Mở trang Shop
        driver.get(CauHinh.BASE_URL + "/Store/Products");

        // Chọn danh mục thứ 2 (bỏ All categories)
        new Select(driver.findElement(By.name("categoryId"))).selectByIndex(1);

        // Nhấn Filter
        CauHinh.bamNut(driver, "Filter");

        // Đợi URL đổi
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.urlContains("categoryId"));

        assertTrue(driver.getCurrentUrl().contains("categoryId"));
        assertTrue(driver.getPageSource().contains("Products"));
    }
}

package tests.user;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test trang chủ dành cho khách hàng.
 */
public class HomeTest extends InKetQua {

    @Test
    @DisplayName("Mở trang chủ")
    void testOpenHomePage() {
        // Mở trang chủ
        driver.get(CauHinh.BASE_URL + "/");

        // Kiểm tra nội dung trang chủ
        assertTrue(driver.getPageSource().contains("Welcome to our online store"));
        assertTrue(driver.getPageSource().contains("Start shopping"));
        assertTrue(driver.getPageSource().contains("Featured products"));
    }

    @Test
    @DisplayName("Bấm Start shopping để vào Shop")
    void testStartShopping() {
        // Mở trang chủ
        driver.get(CauHinh.BASE_URL + "/");

        // Bấm Start shopping
        driver.findElement(By.linkText("Start shopping")).click();

        // Kiểm tra đã vào trang sản phẩm
        assertTrue(driver.getCurrentUrl().contains("Store/Products"));
        assertTrue(driver.getPageSource().contains("Products"));
    }

    @Test
    @DisplayName("Menu trang chủ có Shop, Categories, Cart, Login, Register")
    void testHomeMenuLinks() {
        // Mở trang chủ
        driver.get(CauHinh.BASE_URL + "/");

        // Kiểm tra các link trên menu
        driver.findElement(By.linkText("Shop"));
        driver.findElement(By.linkText("Categories"));
        driver.findElement(By.linkText("Cart"));
        driver.findElement(By.linkText("Login"));
        driver.findElement(By.linkText("Register"));
    }
}

package tests.user;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test giỏ hàng của khách hàng.
 */
public class CartTest extends InKetQua {

    @Test
    @DisplayName("Giỏ hàng trống khi chưa thêm sản phẩm")
    void testEmptyCart() {
        // Mở trang giỏ hàng
        driver.get(CauHinh.BASE_URL + "/Cart");

        // Kiểm tra giỏ hàng trống
        assertTrue(driver.getPageSource().contains("Your Cart"));
        assertTrue(driver.getPageSource().contains("Your cart is empty."));
    }

    @Test
    @DisplayName("Thêm sản phẩm vào giỏ hàng")
    void testAddToCart() {
        // Thêm sản phẩm còn hàng vào giỏ
        CauHinh.themSanPhamVaoGio(driver);

        // Kiểm tra đã vào giỏ hàng và có sản phẩm
        assertTrue(driver.getCurrentUrl().contains("/Cart"));
        assertTrue(driver.getPageSource().contains("Your Cart"));
        assertFalse(driver.getPageSource().contains("Your cart is empty."));
        assertTrue(driver.getPageSource().contains("Total:"));
    }

    @Test
    @DisplayName("Cập nhật số lượng trong giỏ hàng")
    void testUpdateCartQuantity() {
        // Thêm sản phẩm vào giỏ
        CauHinh.themSanPhamVaoGio(driver);

        // Đổi số lượng thành 2
        driver.findElement(By.cssSelector("input[name='quantity']")).clear();
        driver.findElement(By.cssSelector("input[name='quantity']")).sendKeys("2");

        // Nhấn Update
        CauHinh.bamNut(driver, "Update");

        driver.get(CauHinh.BASE_URL + "/Cart");
        assertEquals("2", driver.findElement(By.cssSelector("input[name='quantity']")).getAttribute("value"));
    }

    @Test
    @DisplayName("Xóa sản phẩm khỏi giỏ hàng")
    void testRemoveFromCart() {
        // Thêm sản phẩm vào giỏ
        CauHinh.themSanPhamVaoGio(driver);

        // Nhấn Remove
        CauHinh.bamNut(driver, "Remove");

        // Đợi thông báo giỏ hàng trống
        driver.findElement(By.xpath("//*[contains(.,'Your cart is empty.')]"));
        assertTrue(driver.getPageSource().contains("Your cart is empty."));
    }

    @Test
    @DisplayName("Không checkout được khi chưa đăng nhập")
    void testCannotCheckoutWhenNotLoggedIn() {
        // Thêm sản phẩm vào giỏ khi chưa đăng nhập
        CauHinh.themSanPhamVaoGio(driver);

        String noiDung = driver.findElement(By.tagName("body")).getText();
        assertTrue(noiDung.contains("login as customer to checkout"));
        assertEquals(0, driver.findElements(By.xpath("//button[contains(.,'Checkout')]")).size());
    }
}

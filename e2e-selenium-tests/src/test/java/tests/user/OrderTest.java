package tests.user;

import tests.common.CauHinh;
import tests.common.InKetQua;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test đặt hàng và xem đơn hàng của khách hàng.
 */
public class OrderTest extends InKetQua {

    @Test
    @DisplayName("Không xem được đơn hàng khi chưa đăng nhập")
    void testCannotViewOrdersWhenNotLoggedIn() {
        // Mở trang đơn hàng khi chưa đăng nhập
        driver.get(CauHinh.BASE_URL + "/Orders");

        // Hệ thống chuyển về trang login khách hàng
        assertTrue(driver.getCurrentUrl().contains("Account/Login"));
        assertTrue(driver.getPageSource().contains("Customer Login"));
    }

    @Test
    @DisplayName("Khách hàng đã đăng nhập xem danh sách đơn hàng trống")
    void testViewEmptyOrdersWhenLoggedIn() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký tài khoản mới (chưa có đơn)
        CauHinh.dangKyKhach(driver, username, "123456");

        // Mở trang đơn hàng
        driver.get(CauHinh.BASE_URL + "/Orders");

        assertTrue(driver.getPageSource().contains("My Orders"));
        assertTrue(driver.getPageSource().contains("You have no orders yet."));
    }

    @Test
    @DisplayName("Đặt hàng thành công")
    void testCheckoutSuccessfully() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký tài khoản khách hàng
        CauHinh.dangKyKhach(driver, username, "123456");

        // Thêm sản phẩm còn hàng vào giỏ
        CauHinh.themSanPhamVaoGio(driver);

        // Nhấn Checkout
        CauHinh.bamNut(driver, "Checkout");

        // Đợi sang trang chi tiết đơn hàng
        driver.findElement(By.xpath("//h2[contains(text(),'Order #')]"));
        assertTrue(driver.getCurrentUrl().contains("Orders/Details"));
        assertTrue(driver.getPageSource().contains("Order #"));
    }

    @Test
    @DisplayName("Xem danh sách đơn hàng sau khi đặt hàng")
    void testViewOrderListAfterCheckout() {
        String username = "user" + System.currentTimeMillis();

        // Đăng ký, thêm giỏ, đặt hàng
        CauHinh.dangKyKhach(driver, username, "123456");
        CauHinh.themSanPhamVaoGio(driver);
        CauHinh.bamNut(driver, "Checkout");
        driver.findElement(By.xpath("//h2[contains(text(),'Order #')]"));

        // Mở danh sách đơn hàng
        driver.get(CauHinh.BASE_URL + "/Orders");

        // Kiểm tra có đơn hàng
        assertTrue(driver.getPageSource().contains("My Orders"));
        assertTrue(driver.findElements(By.linkText("Details")).size() > 0);
    }
}

package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.time.Duration;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test quản lý sản phẩm.
 */
public class ProductTest extends InKetQuaAdmin {

    @Test
    @DisplayName("Xem danh sách sản phẩm")
    void testViewProductList() {
        // Mở trang quản lý sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Kiểm tra trang danh sách đã mở
        assertTrue(driver.getPageSource().contains("Products"));
        assertTrue(driver.getPageSource().contains("Create new product"));
    }

    @Test
    @DisplayName("Tạo sản phẩm mới")
    void testCreateProduct() {
        String tenSanPham = "Test Product " + System.currentTimeMillis();

        // Mở trang tạo sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts/Create");

        // Nhập tên sản phẩm
        driver.findElement(By.id("Name")).sendKeys(tenSanPham);

        // Nhập mô tả
        driver.findElement(By.id("Description")).sendKeys("San pham dung de test");

        // Nhập giá
        driver.findElement(By.id("Price")).clear();
        driver.findElement(By.id("Price")).sendKeys("20");

        // Chọn danh mục đầu tiên
        new Select(driver.findElement(By.id("CategoryId"))).selectByIndex(0);

        // Nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Đợi quay về danh sách và thấy sản phẩm mới
        driver.findElement(By.xpath("//td[contains(text(),'" + tenSanPham + "')]"));
        assertFalse(driver.getCurrentUrl().contains("Create"));
        assertTrue(driver.getPageSource().contains(tenSanPham));
    }

    @Test
    @DisplayName("Sửa sản phẩm")
    void testEditProduct() {
        String tenCu = "ProdOld " + System.currentTimeMillis();
        String tenMoi = "ProdNew " + System.currentTimeMillis();

        // Tạo sản phẩm trước
        driver.get(CauHinh.BASE_URL + "/AdminProducts/Create");
        driver.findElement(By.id("Name")).sendKeys(tenCu);
        driver.findElement(By.id("Price")).clear();
        driver.findElement(By.id("Price")).sendKeys("10");
        new Select(driver.findElement(By.id("CategoryId"))).selectByIndex(0);
        CauHinh.bamNut(driver, "Save");
        driver.findElement(By.xpath("//td[contains(text(),'" + tenCu + "')]"));

        // Bấm Edit đúng dòng vừa tạo
        driver.findElement(By.xpath("//tr[td[contains(text(),'" + tenCu + "')]]//a[text()='Edit']")).click();

        // Đổi tên sản phẩm
        driver.findElement(By.id("Name")).clear();
        driver.findElement(By.id("Name")).sendKeys(tenMoi);
        driver.findElement(By.id("Price")).clear();
        driver.findElement(By.id("Price")).sendKeys("10");

        // Nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Đợi quay về danh sách
        driver.findElement(By.xpath("//h2[text()='Products']"));
        assertTrue(driver.getPageSource().contains(tenMoi));
    }

    @Test
    @DisplayName("Xóa sản phẩm")
    void testDeleteProduct() {
        String tenSanPham = "Delete Product " + System.currentTimeMillis();

        // Tạo sản phẩm để xóa
        driver.get(CauHinh.BASE_URL + "/AdminProducts/Create");
        driver.findElement(By.id("Name")).sendKeys(tenSanPham);
        driver.findElement(By.id("Price")).clear();
        driver.findElement(By.id("Price")).sendKeys("10");
        new Select(driver.findElement(By.id("CategoryId"))).selectByIndex(0);
        CauHinh.bamNut(driver, "Save");
        driver.findElement(By.xpath("//td[contains(text(),'" + tenSanPham + "')]"));

        // Bấm Delete đúng dòng vừa tạo
        driver.findElement(By.xpath("//tr[td[contains(text(),'" + tenSanPham + "')]]//a[text()='Delete']")).click();

        // Xác nhận xóa
        CauHinh.bamNut(driver, "Delete");

        // Kiểm tra sản phẩm không còn trên danh sách
        driver.findElement(By.xpath("//h2[contains(text(),'Products')]"));
        assertFalse(driver.getPageSource().contains(tenSanPham));
    }

    @Test
    @DisplayName("Lọc sản phẩm theo danh mục")
    void testFilterProductByCategory() {
        // Mở trang sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts");

        // Chọn danh mục thứ 2
        new Select(driver.findElement(By.name("categoryId"))).selectByIndex(1);

        CauHinh.bamNut(driver, "Filter");

        // Đợi URL đổi sau khi lọc
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.urlContains("categoryId"));

        assertTrue(driver.getCurrentUrl().contains("categoryId"));
        assertTrue(driver.getPageSource().contains("Products"));
    }

    @Test
    @DisplayName("Không tạo được sản phẩm khi thiếu tên")
    void testCreateProductWithoutName() {
        // Mở trang tạo sản phẩm
        driver.get(CauHinh.BASE_URL + "/AdminProducts/Create");

        // Không nhập Name, nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Vẫn ở trang Create
        assertTrue(driver.getCurrentUrl().contains("Create"));
    }
}

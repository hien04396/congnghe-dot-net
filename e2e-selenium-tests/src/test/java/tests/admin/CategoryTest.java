package tests.admin;

import tests.common.CauHinh;
import tests.common.InKetQuaAdmin;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

/**
 * Test quản lý danh mục sản phẩm.
 */
public class CategoryTest extends InKetQuaAdmin {

    @Test
    @DisplayName("Xem danh sách danh mục")
    void testViewCategoryList() {
        // Mở trang quản lý danh mục
        driver.get(CauHinh.BASE_URL + "/AdminProductCategories");

        // Kiểm tra trang danh sách đã mở
        assertTrue(driver.getPageSource().contains("Product Categories"));
        assertTrue(driver.getPageSource().contains("Create new category"));
    }

    @Test
    @DisplayName("Tạo danh mục mới")
    void testCreateCategory() {
        String tenDanhMuc = "Test Category " + System.currentTimeMillis();

        // Mở trang tạo danh mục
        driver.get(CauHinh.BASE_URL + "/AdminProductCategories/Create");

        // Nhập tên danh mục
        driver.findElement(By.id("Name")).sendKeys(tenDanhMuc);

        // Nhập mô tả
        driver.findElement(By.id("Description")).sendKeys("Danh muc dung de test");

        // Nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Đợi quay về danh sách
        driver.findElement(By.xpath("//h2[contains(text(),'Product Categories')]"));
        assertFalse(driver.getCurrentUrl().contains("Create"));
        assertTrue(driver.getPageSource().contains(tenDanhMuc));
    }

    @Test
    @DisplayName("Sửa danh mục")
    void testEditCategory() {
        String tenCu = "CatOld " + System.currentTimeMillis();
        String tenMoi = "CatNew " + System.currentTimeMillis();

        // Tạo danh mục trước để có dữ liệu sửa
        driver.get(CauHinh.BASE_URL + "/AdminProductCategories/Create");
        driver.findElement(By.id("Name")).sendKeys(tenCu);
        CauHinh.bamNut(driver, "Save");
        driver.findElement(By.xpath("//td[contains(text(),'" + tenCu + "')]"));

        // Bấm Edit đúng dòng vừa tạo
        driver.findElement(By.xpath("//tr[td[contains(text(),'" + tenCu + "')]]//a[text()='Edit']")).click();

        // Xóa tên cũ và nhập tên mới
        driver.findElement(By.id("Name")).clear();
        driver.findElement(By.id("Name")).sendKeys(tenMoi);

        // Nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Kiểm tra tên mới xuất hiện, tên cũ không còn
        driver.findElement(By.xpath("//h2[contains(text(),'Product Categories')]"));
        assertTrue(driver.getPageSource().contains(tenMoi));
        assertFalse(driver.getPageSource().contains(tenCu));
    }

    @Test
    @DisplayName("Xóa danh mục")
    void testDeleteCategory() {
        String tenDanhMuc = "Delete Category " + System.currentTimeMillis();

        // Tạo danh mục để xóa
        driver.get(CauHinh.BASE_URL + "/AdminProductCategories/Create");
        driver.findElement(By.id("Name")).sendKeys(tenDanhMuc);
        CauHinh.bamNut(driver, "Save");
        driver.findElement(By.xpath("//td[contains(text(),'" + tenDanhMuc + "')]"));

        // Bấm Delete đúng dòng vừa tạo
        driver.findElement(By.xpath("//tr[td[contains(text(),'" + tenDanhMuc + "')]]//a[text()='Delete']")).click();

        // Xác nhận xóa
        CauHinh.bamNut(driver, "Delete");

        // Kiểm tra danh mục không còn trên danh sách
        driver.findElement(By.xpath("//h2[contains(text(),'Product Categories')]"));
        assertFalse(driver.getPageSource().contains(tenDanhMuc));
    }

    @Test
    @DisplayName("Không tạo được danh mục khi thiếu tên")
    void testCreateCategoryWithoutName() {
        // Mở trang tạo danh mục
        driver.get(CauHinh.BASE_URL + "/AdminProductCategories/Create");

        // Không nhập Name, nhấn Save
        CauHinh.bamNut(driver, "Save");

        // Vẫn ở trang Create, chưa tạo thành công
        assertTrue(driver.getCurrentUrl().contains("Create"));
    }
}

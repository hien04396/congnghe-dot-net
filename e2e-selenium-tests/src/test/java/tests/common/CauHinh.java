package tests.common;

import java.time.Duration;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;

/**
 * File cấu hình dùng chung cho tất cả test.
 */
public class CauHinh {

    public static final String BASE_URL = System.getProperty("baseUrl", "http://127.0.0.1:5000");

    // Tài khoản admin mặc định của hệ thống
    public static final String ADMIN_USERNAME = "admin";
    public static final String ADMIN_PASSWORD = "admin";

    /**
     * Mở trình duyệt Chrome.
     */
    public static WebDriver moChrome() {
        ChromeOptions options = new ChromeOptions();
        // options.addArguments("--headless=new");
        options.addArguments("--disable-gpu");
        options.addArguments("--no-sandbox");
        options.addArguments("--disable-dev-shm-usage");

        WebDriver driver = new ChromeDriver(options);
        driver.manage().window().maximize();
        driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
        return driver;
    }

    /**
     * Đăng nhập trang Admin.
     */
    public static void dangNhapAdmin(WebDriver driver) {
        driver.get(BASE_URL + "/admin");
        driver.findElement(By.id("Username")).sendKeys(ADMIN_USERNAME);
        driver.findElement(By.id("Password")).sendKeys(ADMIN_PASSWORD);
        driver.findElement(By.cssSelector("button[type='submit']")).click();
        driver.findElement(By.xpath("//h2[contains(text(),'Admin Dashboard')]"));
    }

    /**
     * Đăng ký tài khoản khách hàng mới.
     */
    public static void dangKyKhach(WebDriver driver, String username, String password) {
        driver.get(BASE_URL + "/Account/Register");
        driver.findElement(By.id("Username")).sendKeys(username);
        driver.findElement(By.id("Password")).sendKeys(password);
        driver.findElement(By.id("ConfirmPassword")).sendKeys(password);
        bamNut(driver, "Register");
        driver.findElement(By.xpath("//*[contains(text(),'Hello, " + username + "')]"));
    }

    /**
     * Đăng nhập tài khoản khách hàng.
     */
    public static void dangNhapKhach(WebDriver driver, String username, String password) {
        driver.get(BASE_URL + "/Account/Login");
        driver.findElement(By.id("Username")).sendKeys(username);
        driver.findElement(By.id("Password")).sendKeys(password);
        bamNut(driver, "Login");
        driver.findElement(By.xpath("//*[contains(text(),'Hello, " + username + "')]"));
    }

    public static void bamNut(WebDriver driver, String tenNut) {
        driver.findElement(By.xpath("//button[contains(.,'" + tenNut + "')]")).click();
    }

    public static void dangXuatKhach(WebDriver driver) {
        bamNut(driver, "Logout");
        driver.findElement(By.linkText("Login"));
    }

    public static void themSanPhamVaoGio(WebDriver driver) {
        driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(2));
        try {
            for (int trang = 1; trang <= 10; trang++) {
                driver.get(BASE_URL + "/Store/Products?page=" + trang);
                int soSanPham = driver.findElements(By.linkText("View details")).size();
                if (soSanPham == 0) {
                    break;
                }
                for (int i = 0; i < soSanPham; i++) {
                    driver.get(BASE_URL + "/Store/Products?page=" + trang);
                    driver.findElements(By.linkText("View details")).get(i).click();
                    if (driver.getPageSource().contains("Out of stock")) {
                        continue;
                    }
                    bamNut(driver, "Add to cart");
                    driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
                    driver.findElement(By.xpath("//h2[contains(text(),'Your Cart')]"));
                    return;
                }
            }
        } finally {
            driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
        }
        throw new IllegalStateException("Không tìm thấy sản phẩm còn hàng để thêm vào giỏ.");
    }
}

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

    // Địa chỉ website. Đổi bằng lệnh: mvn test -DbaseUrl=http://127.0.0.1:5088
    public static final String BASE_URL = System.getProperty("baseUrl", "http://127.0.0.1:5000");

    // Tài khoản admin mặc định của hệ thống
    public static final String ADMIN_USERNAME = "admin";
    public static final String ADMIN_PASSWORD = "admin";

    /**
     * Mở trình duyệt Chrome.
     */
    public static WebDriver moChrome() {
        ChromeOptions options = new ChromeOptions();
        // Xóa dòng dưới nếu muốn nhìn thấy cửa sổ Chrome khi test
        // options.addArguments("--headless=new");
        options.addArguments("--disable-gpu");
        options.addArguments("--no-sandbox");
        options.addArguments("--disable-dev-shm-usage");

        WebDriver driver = new ChromeDriver(options);
        // Maximize sau khi mở cửa sổ. Không dùng --window-size=1920,1080
        // vì trên Mac Retina viewport bị thu nhỏ, Bootstrap hiện layout mobile.
        driver.manage().window().maximize();
        // Đợi tối đa 10 giây khi tìm một phần tử trên trang
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
        // Đợi Dashboard hiện ra để chắc chắn đã đăng nhập xong
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
        // Đợi trang chủ hiện tên khách hàng
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

    /**
     * Bấm nút theo chữ trên nút.
     * Không dùng button[type=submit] vì menu có nút Logout cũng là submit.
     */
    public static void bamNut(WebDriver driver, String tenNut) {
        driver.findElement(By.xpath("//button[contains(.,'" + tenNut + "')]")).click();
    }

    /**
     * Đăng xuất khách hàng, đợi thấy link Login.
     */
    public static void dangXuatKhach(WebDriver driver) {
        bamNut(driver, "Logout");
        driver.findElement(By.linkText("Login"));
    }

    /**
     * Thêm một sản phẩm còn hàng vào giỏ.
     * Có thể phải lật trang vì sản phẩm mới tạo (hết hàng) đang đứng đầu danh sách.
     */
    public static void themSanPhamVaoGio(WebDriver driver) {
        // Giảm thời gian đợi khi kiểm tra sản phẩm hết hàng
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
                    // Trang chi tiết ghi "Out of stock" khi hết hàng
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

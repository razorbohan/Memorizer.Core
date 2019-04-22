/// <binding ProjectOpened='browser-sync, watch:scss, watch:js' />
var gulp = require("gulp"),
    sass = require("gulp-sass"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify"),
    browserSync = require("browser-sync");

var paths = {
    wwwroot: "./wwwroot/"
};

paths.js = paths.wwwroot + "js/src/**/*.js";
paths.minJs = paths.wwwroot + "js/dist/**/*.js";
paths.concatJsDest = paths.wwwroot + "js/dist";
paths.css = paths.wwwroot + "css/**/*.css";
paths.minCss = paths.wwwroot + "css/**/*.min.css";
paths.concatCssDest = paths.wwwroot + "css";
paths.scss = paths.wwwroot + "scss/**/*.scss";

paths.views = "./Views/**/*.cshtml";
paths.pages = "./Areas/Identity/Pages/**/*.cshtml";

gulp.task("browser-sync", () => {
    browserSync.init({
        proxy: "https://localhost:44367" ,
        files: [paths.css, paths.minJs, paths.views, paths.pages],
        ghostMode: false,
        logFileChanges: true,
        logLevel: "info"
    });
});

gulp.task("scss", () => {
    return gulp.src(paths.scss)
        .pipe(sass())
        .pipe(sass().on("error", sass.logError))
        .pipe(gulp.dest(paths.wwwroot + "css"));
});

gulp.task("min:css", () => {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat("site.css"))
        .pipe(gulp.dest(paths.concatCssDest))
        .pipe(rename("site.min.css"))
        .pipe(cssmin())
        .pipe(gulp.dest(paths.concatCssDest));
});

gulp.task("min:js", () => {
    return gulp.src(paths.js)
        .pipe(concat("site.js"))
        .pipe(gulp.dest(paths.concatJsDest))
        .pipe(rename("site.min.js"))
        .pipe(uglify())
        .pipe(gulp.dest(paths.concatJsDest));
});

gulp.task("watch:scss", function () {
    gulp.watch(paths.scss, gulp.series("scss", "min:css"));
});

gulp.task("watch:js", function () {
    gulp.watch(paths.js, gulp.series("min:js"));
});
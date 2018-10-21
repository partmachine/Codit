/// <binding AfterBuild='default' />
var gulp = require('gulp'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    rename = require('gulp-rename'),
    sass = require('gulp-ruby-sass'),
    autoprefixer = require('gulp-autoprefixer'),
    browserSync = require('browser-sync').create();

var DEST = 'wwwroot';

gulp.task('scripts', function () {
    return gulp.src([
        './wwwsrc/js/helpers/*.js',
        'wwwsrc/js/*.js',
    ])
        .pipe(concat('site.js'))
        .pipe(gulp.dest(DEST + '/js'))
        .pipe(rename({ suffix: '.min' }))
        .pipe(uglify())
        .pipe(gulp.dest(DEST + '/js'))
        .pipe(browserSync.stream());
});



// TODO: Maybe we can simplify how sass compile the minify and unminify version
var compileSASS = function (filename, theme, options) {


    var src = './wwwsrc/scss/' + filename + '.' + theme + '.scss';
    var dst = filename + '.' + theme + '.css';

    if (options.style == 'compressed') {
        dst = filename + '.' + theme + '.min.css';
    }

    if (theme === '' || theme == null) {
        src = './wwwsrc/scss/' + filename + '.scss';
        dst = filename + '.css';
        if (options.style == 'compressed') {
            dst = filename + '.min.css';
        }
    }
    
    console.log('compiling ' + src);
    
    return sass(src, options)
        .pipe(autoprefixer('last 2 versions', '> 5%'))
        .pipe(concat(dst))
        .pipe(gulp.dest(DEST + '/css'))
        .pipe(browserSync.stream());
    
};

gulp.task('sass', function () {   

    compileSASS('custom',null, {});
    return compileSASS('custom', 'dark', {});
});

gulp.task('sass-minify', function () {
    compileSASS('custom', null, { style: 'compressed' });
    return compileSASS('custom', 'dark', { style: 'compressed' });
});

// Default Task
gulp.task('default', ['scripts', 'sass','sass-minify']);
module.exports = function (grunt) {

    grunt.initConfig({
        less: {
            development: {
                files: {
                    "Content/Styles/Site.css": "Content/Styles/Site.less"
                }
            }
        },
        watch: {
            styles: {
                files: ['Content/Styles/**/*a.less'],
                tasks: ['less'],
                options: {
                    nospawn: true
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-less');

    grunt.registerTask('default', ['less']);
};

/** @type {import('tailwindcss').Config} */
/**Make sure to save the css file with only UTF-8 without signature*/
export default {
    content: [
        "./**/*.{razor,html,cshtml}",
        "./**/(Layout|Pages)/*.{razor,html,cshtml}", // Include only Layout and Pages folders
        "./node_modules/flowbite/**/*.js"
    ],
    theme: {
        extend: {},
    },
    plugins: [
    ],
}
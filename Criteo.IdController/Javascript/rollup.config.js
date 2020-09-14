import typescript from 'rollup-plugin-typescript2';

function generateRollupConfig(tsInput, jsOutput) {
    return {
        input: tsInput,
        output: {
            file: jsOutput,
            format: 'esm'
        },
        plugins: [
            typescript(/*{ plugin options }*/)
        ]
    }
}

const widgetConfig = generateRollupConfig('./src/widget.ts', '../wwwroot/js/widget.js');
const initialBannerConfig = generateRollupConfig('./src/initialBanner.ts', '../wwwroot/js/initialBanner.js');
const optionsBannerConfig = generateRollupConfig('./src/optionsBanner.ts', '../wwwroot/js/optionsBanner.js');
const learnMoreSiteConfig = generateRollupConfig('./src/learnMoreSite.ts', '../wwwroot/js/learnMoreSite.js');

export default [
    widgetConfig,
    initialBannerConfig,
    optionsBannerConfig,
    learnMoreSiteConfig
]
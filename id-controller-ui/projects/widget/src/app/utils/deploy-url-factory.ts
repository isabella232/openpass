export const deployUrl = () => {
  const openpassScripts = document.querySelector('script[openpass]') as HTMLScriptElement;
  return new URL(openpassScripts.src).origin;
};

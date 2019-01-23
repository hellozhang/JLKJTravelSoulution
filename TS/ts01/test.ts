/**
 * Demo
 */
class Demo {
    a: number;
    b: number;

    constructor(a: number, b: number) {
        this.a = a;
        this.b = b;
    }

    sum(): number {
        return this.a + this.b;
    }
}


class ext extends Demo {
    c: any;
    constructor(c: string) {
        super(c);
       
    }
  sum():number
  {
      return this.a;
  }

}


export { Demo };


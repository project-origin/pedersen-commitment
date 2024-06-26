use core::slice;

use curve25519_dalek::scalar::Scalar;
use sha3::Sha3_512;

use crate::reref;

#[no_mangle]
pub unsafe extern "C" fn scalar_new(bytes: *const u8) -> *const Scalar {
    let bytes = unsafe { slice::from_raw_parts(bytes, 32) };
    let bytes = bytes.try_into().unwrap();
    let scalar = Scalar::from_bytes_mod_order(bytes);
    Box::into_raw(Box::new(scalar))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_random() -> *const Scalar {
    let scalar = Scalar::random(&mut rand::thread_rng());
    Box::into_raw(Box::new(scalar))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_hash_from_bytes(bytes: *const u8, len: i32) -> *const Scalar {
    let bytes = unsafe { slice::from_raw_parts(bytes, len as usize) };
    let scalar = Scalar::hash_from_bytes::<Sha3_512>(bytes);
    Box::into_raw(Box::new(scalar))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_to_bytes(this: *mut Scalar, dst: *mut u8) {
    let this = &*this;
    let dst = slice::from_raw_parts_mut(dst, 32);
    let src = this.as_bytes();
    dst.clone_from_slice(src);
}

#[no_mangle]
pub unsafe extern "C" fn scalar_spill_guts(this: *mut Scalar) {
    let this = reref(this);
    println!("{this:?}");
}

#[no_mangle]
pub unsafe extern "C" fn scalar_equals(lhs: *const Scalar, rhs: *const Scalar) -> bool {
    let lhs = &*lhs;
    let rhs = &*rhs;
    lhs == rhs
}

#[no_mangle]
pub unsafe extern "C" fn scalar_add(lhs: *const Scalar, rhs: *const Scalar) -> *const Scalar {
    let lhs = &*lhs;
    let rhs = &*rhs;
    Box::into_raw(Box::new(lhs + rhs))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_sub(lhs: *const Scalar, rhs: *const Scalar) -> *const Scalar {
    let lhs = &*lhs;
    let rhs = &*rhs;
    Box::into_raw(Box::new(lhs - rhs))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_negate(this: *const Scalar) -> *const Scalar {
    let this = &*this;
    Box::into_raw(Box::new(-this))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_mul(lhs: *const Scalar, rhs: *const Scalar) -> *const Scalar {
    let lhs = &*lhs;
    let rhs = &*rhs;
    Box::into_raw(Box::new(lhs * rhs))
}

#[no_mangle]
pub extern "C" fn scalar_sum(args: *const *const Scalar, len: i32) -> *const Scalar {
    let args = unsafe { std::slice::from_raw_parts(args, len as usize) };
    Box::into_raw(Box::new(args.iter().map(|&ptr| unsafe { *ptr }).sum()))
}

#[no_mangle]
pub unsafe extern "C" fn scalar_free(this: *mut Scalar) {
    if this.is_null() {
        return;
    }
    unsafe {
        drop(Box::from_raw(this));
    }
}
